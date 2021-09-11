using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class DataCollector : IDataCollector {
        private readonly IDataPresenter DataPresenter;
        private readonly IPostingCollector PostingCollector;
        private readonly ISummaryCalculator SummaryCalculator;
        private readonly IAverageCalculator AverageCalculator;
        private readonly IMonthlyDeltaCalculator MonthlyDeltaCalculator;

        public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator, IAverageCalculator averageCalculator, IMonthlyDeltaCalculator monthlyDeltaCalculator) {
            DataPresenter = dataPresenter;
            PostingCollector = postingCollector;
            SummaryCalculator = summaryCalculator;
            AverageCalculator = averageCalculator;
            MonthlyDeltaCalculator = monthlyDeltaCalculator;
        }

        public async Task CollectAndShowAsync(IContainer container, bool isIntegrationTest) {
            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();

            var specialCluesSecret = await secretRepository.GetAsync(new SpecialCluesSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var specialClues = specialCluesSecret.Cast<ISpecialClue>().ToList();

            var allPostings = await PostingCollector.CollectPostingsAsync(container, isIntegrationTest, specialClues);

            var postingClassificationsSecret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var postingClassifications = postingClassificationsSecret.Cast<IPostingClassification>().ToList();

            await SummaryCalculator.CalculateAndShowSummaryAsync(allPostings, postingClassifications);
            await AverageCalculator.CalculateAndShowAverageAsync(allPostings, postingClassifications);
            await MonthlyDeltaCalculator.CalculateAndShowMonthlyDeltaAsync(allPostings, postingClassifications, specialClues);
        }
    }
}