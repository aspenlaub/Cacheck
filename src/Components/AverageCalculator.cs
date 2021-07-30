using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class AverageCalculator : IAverageCalculator {
        private readonly IDataPresenter vDataPresenter;
        private readonly IPostingAggregator vPostingAggregator;

        public AverageCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator) {
            vDataPresenter = dataPresenter;
            vPostingAggregator = postingAggregator;
        }

        public async Task CalculateAndShowAverageAsync(IContainer container, IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
            var errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = vPostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos).OrderBy(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var keyLength = detailedAggregation.Select(result => result.Key.CombinedClassification.Length).Max();
            foreach (var s in
                from result in detailedAggregation
                let s = (result.Value / 12).ToString("0.##")
                select $"Average {result.Key.CombinedClassification.PadRight(keyLength)}: {s}") {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Average, s);
            }

            var classificationAverageList = detailedAggregation.Select(
                result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value / 12 }
            ).Cast<ITypeItemSum>().ToList();
            await container.Resolve<IClassificationAveragePresenter>().PresentAsync(classificationAverageList);
        }
    }
}
