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
                        select $"Sum {result.Key}: {s}") {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, s);
            }

            errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = vPostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos).OrderBy(a => a.Key).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary);

            int keyLength;
            if (detailedAggregation.Any()) {
                keyLength = detailedAggregation.Select(a => a.Key.Length).Max();
                foreach (var s in
                    from result in detailedAggregation
                    let s = result.Value.ToString("0.##")
                    select $"Sum {result.Key.PadRight(keyLength)}: {s}") {
                    await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, s);
                }
            }

            await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary);

            foreach (var info in errorsAndInfos.Infos) {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.Summary, info);
            }
        }
    }
}
