using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class SummaryCalculator : ISummaryCalculator {
        private readonly IDataPresenter vDataPresenter;
        private readonly IPostingAggregator vPostingAggregator;

        public SummaryCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator) {
            vDataPresenter = dataPresenter;
            vPostingAggregator = postingAggregator;
        }

        public async Task CalculateAndShowSummaryAsync(IContainer container, IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
            var errorsAndInfos = new ErrorsAndInfos();
            var pureDebitCreditAggregation = vPostingAggregator.AggregatePostings(allPostings, new List<IPostingClassification> {
                new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
                new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
            }, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            foreach (var s in
                        from result in pureDebitCreditAggregation
                        let s = result.Value.ToString("0.##")
                        select $"Delta {result.Key}: {s}") {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, s);
            }

            var overallSumList = pureDebitCreditAggregation.Select(
                a => new TypeItemSum { Item = a.Key, Sum = a.Value }
            ).Cast<ITypeItemSum>().ToList();
            await container.Resolve<IOverallSumPresenter>().PresentAsync(overallSumList);

            errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = vPostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos).OrderBy(result => result.Key).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary);

            int keyLength;
            if (detailedAggregation.Any()) {
                keyLength = detailedAggregation.Select(result => result.Key.Length).Max();
                foreach (var s in
                    from result in detailedAggregation
                    let s = result.Value.ToString("0.##")
                    select $"Delta {result.Key.PadRight(keyLength)}: {s}") {
                    await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, s);
                }

                var classificationSumList = detailedAggregation.Select(
                    result => new TypeItemSum { Item = result.Key, Sum = result.Value }
                ).Cast<ITypeItemSum>().ToList();
                await container.Resolve<IClassificationSumPresenter>().PresentAsync(classificationSumList);
            }

            await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary);

            foreach (var info in errorsAndInfos.Infos) {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, info);
            }
        }
    }
}
