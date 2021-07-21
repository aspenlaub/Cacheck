using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class DataCollector : IDataCollector {
        private readonly IDataPresenter vDataPresenter;
        private readonly IPostingCollector vPostingCollector;
        private readonly ISummaryCalculator vSummaryCalculator;
        private readonly IAverageCalculator vAverageCalculator;

        public DataCollector(IDataPresenter dataPresenter, IPostingCollector postingCollector, ISummaryCalculator summaryCalculator, IAverageCalculator averageCalculator) {
            vDataPresenter = dataPresenter;
            vPostingCollector = postingCollector;
            vSummaryCalculator = summaryCalculator;
            vAverageCalculator = averageCalculator;
        }

        public async Task CollectAndShowAsync(IContainer container, bool isIntegrationTest) {
            var allPostings = await vPostingCollector.CollectPostingsAsync(container, isIntegrationTest);

            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();
            var secret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var postingClassifications = secret.Cast<IPostingClassification>().ToList();

            await vSummaryCalculator.CalculateAndShowSummaryAsync(container, allPostings, postingClassifications);
            await vAverageCalculator.CalculateAndShowAverageAsync(container, allPostings, postingClassifications);
        }
    }
}