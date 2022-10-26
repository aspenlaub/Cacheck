using System;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using IContainer = Autofac.IContainer;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class DataCollector : IDataCollector {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingCollector _PostingCollector;
    private readonly ISummaryCalculator _SummaryCalculator;
    private readonly IAverageCalculator _AverageCalculator;
    private readonly IMonthlyDeltaCalculator _MonthlyDeltaCalculator;
    private readonly IClassifiedPostingsCalculator _ClassifiedPostingsCalculator;
    private readonly ICalculationLogger _CalculationLogger;

    public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator, IAverageCalculator averageCalculator,
        IMonthlyDeltaCalculator monthlyDeltaCalculator, IClassifiedPostingsCalculator classifiedPostingsCalculator, ICalculationLogger calculationLogger) {
        _DataPresenter = dataPresenter;
        _PostingCollector = postingCollector;
        _SummaryCalculator = summaryCalculator;
        _AverageCalculator = averageCalculator;
        _MonthlyDeltaCalculator = monthlyDeltaCalculator;
        _ClassifiedPostingsCalculator = classifiedPostingsCalculator;
        _CalculationLogger = calculationLogger;
    }

    public async Task CollectAndShowAsync(IContainer container, bool isIntegrationTest) {
        var errorsAndInfos = new ErrorsAndInfos();
        var secretRepository = container.Resolve<ISecretRepository>();

        _CalculationLogger.ClearLogs();

        var allPostings = await _PostingCollector.CollectPostingsAsync(container, isIntegrationTest);

        var postingClassificationsSecret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var postingClassifications = postingClassificationsSecret.Cast<IPostingClassification>().ToList();

        var inverseClassificationsSecret = await secretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var inverseClassifications = inverseClassificationsSecret.Cast<IInverseClassificationPair>().ToList();

        await _SummaryCalculator.CalculateAndShowSummaryAsync(allPostings, postingClassifications, inverseClassifications);
        await _AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications);

        await _MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications);

        await _ClassifiedPostingsCalculator.CalculateAndShowClassifiedPostingsAsync(allPostings, postingClassifications, DateTime.Now.AddYears(-1), 70);

        _CalculationLogger.Flush();
    }
}