using System;
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
    private readonly IClassifiedPostingsCalculator _ClassifiedPostingsCalculator;
    private readonly ICalculationLogger _CalculationLogger;
    private readonly ISecretRepository _SecretRepository;
    private readonly bool _IsIntegrationTest;

    public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator,
            IAverageCalculator averageCalculator, IMonthlyDeltaCalculator monthlyDeltaCalculator, IClassifiedPostingsCalculator classifiedPostingsCalculator,
            ICalculationLogger calculationLogger, ISecretRepository secretRepository, bool isIntegrationTest) {
        _DataPresenter = dataPresenter;
        _DataPresenter.SetDataCollector(this);
        _PostingCollector = postingCollector;
        _SummaryCalculator = summaryCalculator;
        _AverageCalculator = averageCalculator;
        _MonthlyDeltaCalculator = monthlyDeltaCalculator;
        _ClassifiedPostingsCalculator = classifiedPostingsCalculator;
        _CalculationLogger = calculationLogger;
        _SecretRepository = secretRepository;
        _IsIntegrationTest = isIntegrationTest;
    }

    public async Task CollectAndShowAsync() {
        var errorsAndInfos = new ErrorsAndInfos();

        _CalculationLogger.ClearLogs();

        var allPostings = await _PostingCollector.CollectPostingsAsync(_IsIntegrationTest);

        var postingClassificationsSecret = await _SecretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var postingClassifications = postingClassificationsSecret.Cast<IPostingClassification>().ToList();

        var inverseClassificationsSecret = await _SecretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var inverseClassifications = inverseClassificationsSecret.Cast<IInverseClassificationPair>().ToList();

        await _DataPresenter.OnClassificationsFoundAsync(postingClassifications, allPostings, inverseClassifications);

        await _SummaryCalculator.CalculateAndShowSummaryAsync(allPostings, postingClassifications, inverseClassifications);
        await _AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications, inverseClassifications);

        await _MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications);

        var singleClassification = _DataPresenter.SingleClassification();
        var minAmount = singleClassification == "" ? 70 : 10;
        var inverseClassification = inverseClassifications.SingleOrDefault(ic
            => ic.Classification == singleClassification || ic.InverseClassification == singleClassification
        );
        var singleClassificationInverse = inverseClassification == null ? "" :
            inverseClassification.Classification == singleClassification
                ? inverseClassification.InverseClassification
                : inverseClassification.Classification;
        await _ClassifiedPostingsCalculator.CalculateAndShowClassifiedPostingsAsync(allPostings, postingClassifications, DateTime.Now.AddYears(-1), minAmount,
            singleClassification, singleClassificationInverse);

        _CalculationLogger.Flush();
    }
}