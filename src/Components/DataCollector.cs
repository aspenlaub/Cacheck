using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using IContainer = Autofac.IContainer;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class DataCollector : IDataCollector {
        private readonly IDataPresenter DataPresenter;
        private readonly IPostingCollector PostingCollector;
        private readonly ISummaryCalculator SummaryCalculator;
        private readonly IAverageCalculator AverageCalculator;
        private readonly IMonthlyDeltaCalculator MonthlyDeltaCalculator;
        private readonly IClassifiedPostingsCalculator ClassifiedPostingsCalculator;

        public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator, IAverageCalculator averageCalculator,
                IMonthlyDeltaCalculator monthlyDeltaCalculator, IClassifiedPostingsCalculator classifiedPostingsCalculator) {
            DataPresenter = dataPresenter;
            PostingCollector = postingCollector;
            SummaryCalculator = summaryCalculator;
            AverageCalculator = averageCalculator;
            MonthlyDeltaCalculator = monthlyDeltaCalculator;
            ClassifiedPostingsCalculator = classifiedPostingsCalculator;
        }

        public async Task CollectAndShowAsync(IContainer container, bool isIntegrationTest) {
            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();

            var allPostings = await PostingCollector.CollectPostingsAsync(container, isIntegrationTest);

            var postingClassificationsSecret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var postingClassifications = postingClassificationsSecret.Cast<IPostingClassification>().ToList();

            await SummaryCalculator.CalculateAndShowSummaryAsync(allPostings, postingClassifications);
            await AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications);

            await MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications);

            await ClassifiedPostingsCalculator.CalculateAndShowClassifiedPostingsAsync(allPostings, postingClassifications);
        }
    }
}