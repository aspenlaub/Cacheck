using System;
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
        var now = DateTime.Now;
        if (_LastCallToCollectAndShow > now.AddSeconds(-5)) {
            return;
        }

        _LastCallToCollectAndShow = now;
        var errorsAndInfos = new ErrorsAndInfos();

        _CalculationLogger.ClearLogs();

        var allTimePostings = await _PostingCollector.CollectPostingsAsync(_IsIntegrationTest);
        var maxDate = allTimePostings.Max(p => p.Date);
        var minDate = new DateTime(maxDate.Year - 1, 1, 1);
        var allPostings = allTimePostings.Where(p => p.Date >= minDate).ToList();
        await _DataPresenter.WriteLineAsync($"{allTimePostings.Count(p => p.Date < minDate)} posting/-s removed except for summary tab");

        var postingClassificationsSecret = await _SecretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var postingClassifications = postingClassificationsSecret.OfType<IPostingClassification>().ToList();

        var individualPostingClassifications = await _IndividualPostingClassificationsSource.GetAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }
        postingClassifications.AddRange(individualPostingClassifications.Select(_IndividualPostingClassificationConverter.Convert));

        postingClassifications.AddRange(CreateUnassignedClassifications());

        var inverseClassificationsSecret = await _SecretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var inverseClassifications = inverseClassificationsSecret.OfType<IInverseClassificationPair>().ToList();

        await _DataPresenter.OnClassificationsFoundAsync(postingClassifications, allPostings, inverseClassifications);

        if (!await _SummaryCalculator.CalculateAndShowSummaryAsync(allTimePostings, postingClassifications, inverseClassifications)) {
            _CalculationLogger.Flush();
            return;
        }

        var liquidityPlanClassificationsSecret = await _SecretRepository.GetAsync(new LiquidityPlanClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var liquidityPlanClassifications = liquidityPlanClassificationsSecret.OfType<ILiquidityPlanClassification>().ToList();

        var irregularDebitClassificationsSecret = await _SecretRepository.GetAsync(new IrregularDebitClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var irregularDebitClassifications = irregularDebitClassificationsSecret.OfType<IIrregularDebitClassification>().ToList();

        await _AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications, inverseClassifications,
            liquidityPlanClassifications, irregularDebitClassifications);

        await _MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications);

        await _MonthlyDetailsCalculator.CalculateAndShowMonthlyDetailsAsync(allPostings, postingClassifications);

        var singleClassification = _DataPresenter.SingleClassification();
        var minAmount = singleClassification == "" ? 70 : 10;
        var inverseClassification = inverseClassifications.SingleOrDefault(ic
            => ic.Classification == singleClassification || ic.InverseClassification == singleClassification
        );
        var singleClassificationInverse = inverseClassification == null ? "" :
            inverseClassification.Classification == singleClassification
                ? inverseClassification.InverseClassification
                : inverseClassification.Classification;
        var classifiedPostings = await _ClassifiedPostingsCalculator.CalculateAndShowClassifiedPostingsAsync(allPostings, postingClassifications,
            DateTime.Now.AddYears(-1), minAmount, singleClassification, singleClassificationInverse);
        var eliminationAnalyzerResults = _IndividualPostingEliminationAnalyzer.AnalyzeClassifiedPostings(classifiedPostings);
        foreach(var eliminationAnalyzerResult in eliminationAnalyzerResults) {
            await _DataPresenter.WriteLineAsync(eliminationAnalyzerResult);
        }

        _CalculationLogger.Flush();
    }

    private IEnumerable<PostingClassification> CreateUnassignedClassifications() {
        return new List<PostingClassification> {
            new() { IsUnassigned = true, Classification = "UnassignedDebit", Credit = false, Clue = Guid.NewGuid().ToString() },
            new() { IsUnassigned = true, Classification = "UnassignedCredit", Credit = true, Clue = Guid.NewGuid().ToString() }
        };
    }
}