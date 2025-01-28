﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class DataCollector : IDataCollector {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingCollector _PostingCollector;
    private readonly ISummaryCalculator _SummaryCalculator;
    private readonly IAverageCalculator _AverageCalculator;
    private readonly IMonthlyDeltaCalculator _MonthlyDeltaCalculator;
    private readonly IMonthlyDetailsCalculator _MonthlyDetailsCalculator;
    private readonly IClassifiedPostingsCalculator _ClassifiedPostingsCalculator;
    private readonly ICalculationLogger _CalculationLogger;
    private readonly ISecretRepository _SecretRepository;
    private readonly IIndividualPostingClassificationsSource _IndividualPostingClassificationsSource;
    private readonly IIndividualPostingClassificationConverter _IndividualPostingClassificationConverter;
    private readonly IIndividualPostingEliminationAnalyzer _IndividualPostingEliminationAnalyzer;
    private readonly bool _IsIntegrationTest;

    public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator,
            IAverageCalculator averageCalculator, IMonthlyDeltaCalculator monthlyDeltaCalculator,
            IMonthlyDetailsCalculator monthlyDetailsCalculator, IClassifiedPostingsCalculator classifiedPostingsCalculator,
            ICalculationLogger calculationLogger, ISecretRepository secretRepository, bool isIntegrationTest,
            IIndividualPostingClassificationsSource individualPostingClassificationsSource,
            IIndividualPostingClassificationConverter individualPostingClassificationConverter,
            IIndividualPostingEliminationAnalyzer individualPostingEliminationAnalyzer) {
        _DataPresenter = dataPresenter;
        _DataPresenter.SetDataCollector(this);
        _PostingCollector = postingCollector;
        _SummaryCalculator = summaryCalculator;
        _AverageCalculator = averageCalculator;
        _MonthlyDeltaCalculator = monthlyDeltaCalculator;
        _MonthlyDetailsCalculator = monthlyDetailsCalculator;
        _ClassifiedPostingsCalculator = classifiedPostingsCalculator;
        _CalculationLogger = calculationLogger;
        _SecretRepository = secretRepository;
        _IsIntegrationTest = isIntegrationTest;
        _IndividualPostingClassificationsSource = individualPostingClassificationsSource;
        _IndividualPostingClassificationConverter = individualPostingClassificationConverter;
        _IndividualPostingEliminationAnalyzer = individualPostingEliminationAnalyzer;
    }

    private DateTime _LastCallToCollectAndShow = DateTime.MinValue;

    public async Task CollectAndShowAsync() {
        if (await DidWeJustCollectAndShowAsync()) { return; }

        var errorsAndInfos = new ErrorsAndInfos();

        _CalculationLogger.ClearLogs();

        await _DataPresenter.ClearLines();

        (IList<IPosting> allTimePostings, List<IPosting> allPostings) = await CollectPostingsAsync();

        List<IPostingClassification> postingClassifications = await CollectPostingClassifications(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            _CalculationLogger.Flush();
            return;
        }

        List<IInverseClassificationPair> inverseClassifications = await CollectInverseClassifications(errorsAndInfos);

        await _DataPresenter.OnClassificationsFoundAsync(postingClassifications, allPostings, inverseClassifications, true);

        if (!await _SummaryCalculator.CalculateAndShowSummaryAsync(allTimePostings, postingClassifications, inverseClassifications)) {
            _CalculationLogger.Flush();
            return;
        }

        await CalculateAndShowAverageAsync(errorsAndInfos, allPostings, postingClassifications, inverseClassifications);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            _CalculationLogger.Flush();
            return;
        }

        await _MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications);

        await _MonthlyDetailsCalculator.CalculateAndShowMonthlyDetailsAsync(allPostings, postingClassifications,
            _DataPresenter.MinimumAmount(), _DataPresenter.FromDay(), _DataPresenter.ToDay());

        await DoEliminationAnalysis(inverseClassifications, allPostings, postingClassifications);

        _CalculationLogger.Flush();
    }

    public async Task CollectAndShowMonthlyDetailsAsync() {
        if (await DidWeJustCollectAndShowAsync()) { return; }

        var errorsAndInfos = new ErrorsAndInfos();

        (IList<IPosting> _, List<IPosting> allPostings) = await CollectPostingsAsync();

        List<IPostingClassification> postingClassifications = await CollectPostingClassifications(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            _CalculationLogger.Flush();
            return;
        }

        await _MonthlyDetailsCalculator.CalculateAndShowMonthlyDetailsAsync(allPostings, postingClassifications,
            _DataPresenter.MinimumAmount(), _DataPresenter.FromDay(), _DataPresenter.ToDay());

        _CalculationLogger.Flush();
    }

    private async Task<List<IInverseClassificationPair>> CollectInverseClassifications(IErrorsAndInfos errorsAndInfos) {
        InverseClassifications inverseClassificationsSecret = await _SecretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return new List<IInverseClassificationPair>();
        }

        var inverseClassifications = inverseClassificationsSecret.OfType<IInverseClassificationPair>().ToList();
        return inverseClassifications;
    }

    private async Task CalculateAndShowAverageAsync(IErrorsAndInfos errorsAndInfos, IList<IPosting> allPostings,
                                                          IList<IPostingClassification> postingClassifications, IList<IInverseClassificationPair> inverseClassifications) {
        LiquidityPlanClassifications liquidityPlanClassificationsSecret = await _SecretRepository.GetAsync(new LiquidityPlanClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        var liquidityPlanClassifications = liquidityPlanClassificationsSecret.OfType<ILiquidityPlanClassification>().ToList();

        IrregularDebitClassifications irregularDebitClassificationsSecret = await _SecretRepository.GetAsync(new IrregularDebitClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        var irregularDebitClassifications = irregularDebitClassificationsSecret.OfType<IIrregularDebitClassification>().ToList();

        await _AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications, inverseClassifications,
                    liquidityPlanClassifications, irregularDebitClassifications);
    }

    private async Task<List<IPostingClassification>> CollectPostingClassifications(IErrorsAndInfos errorsAndInfos) {
        PostingClassifications postingClassificationsSecret = await _SecretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return new List<IPostingClassification>(); }

        var postingClassifications = postingClassificationsSecret.OfType<IPostingClassification>().ToList();

        IEnumerable<IIndividualPostingClassification> individualPostingClassifications = await _IndividualPostingClassificationsSource.GetAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return postingClassifications; }

        postingClassifications.AddRange(individualPostingClassifications.Select(_IndividualPostingClassificationConverter.Convert));

        postingClassifications.AddRange(CreateUnassignedClassifications());
        return postingClassifications;
    }

    private async Task<(IList<IPosting> allTimePostings, List<IPosting> allPostings)> CollectPostingsAsync() {
        IList <IPosting> allTimePostings = await _PostingCollector.CollectPostingsAsync(_IsIntegrationTest);
        DateTime maxDate = allTimePostings.Max(p => p.Date);
        var minDate = new DateTime(maxDate.Year - 1, 1, 1);
        var allPostings = allTimePostings.Where(p => p.Date >= minDate).ToList();
        await _DataPresenter.WriteLineAsync($"{allTimePostings.Count(p => p.Date < minDate)} posting/-s removed except for summary tab");
        return (allTimePostings, allPostings);
    }

    private async Task DoEliminationAnalysis(IEnumerable<IInverseClassificationPair> inverseClassifications, IList<IPosting> allPostings,
                                             IList<IPostingClassification> postingClassifications) {
        string singleClassification = _DataPresenter.SingleClassification();
        int minAmount = singleClassification == "" ? 70 : 10;
        IInverseClassificationPair inverseClassification
            = inverseClassifications.SingleOrDefault(
                ic => ic.Classification == singleClassification || ic.InverseClassification == singleClassification
            );
        string singleClassificationInverse = inverseClassification == null ? "" :
            inverseClassification.Classification == singleClassification
                ? inverseClassification.InverseClassification
                : inverseClassification.Classification;
        IList<IClassifiedPosting> classifiedPostings = await _ClassifiedPostingsCalculator.CalculateAndShowClassifiedPostingsAsync(allPostings, postingClassifications,
            DateTime.Now.AddYears(-1), minAmount, singleClassification, singleClassificationInverse);
        IList<string> eliminationAnalyzerResults = _IndividualPostingEliminationAnalyzer.AnalyzeClassifiedPostings(classifiedPostings);
        foreach(string eliminationAnalyzerResult in eliminationAnalyzerResults) {
            await _DataPresenter.WriteLineAsync(eliminationAnalyzerResult);
        }
    }

    private IEnumerable<PostingClassification> CreateUnassignedClassifications() {
        return (List<PostingClassification>) [
            new() { IsUnassigned = true, Classification = "UnassignedDebit", Credit = false, Clue = Guid.NewGuid().ToString() },
            new() { IsUnassigned = true, Classification = "UnassignedCredit", Credit = true, Clue = Guid.NewGuid().ToString() }
        ];
    }

    private async Task<bool> DidWeJustCollectAndShowAsync() {
        DateTime now = DateTime.Now;
        if (_LastCallToCollectAndShow > now.AddSeconds(-5)) {
            return true;
        }

        _LastCallToCollectAndShow = now;

        _CalculationLogger.ClearLogs();

        await _DataPresenter.ClearLines();

        return false;
    }
}