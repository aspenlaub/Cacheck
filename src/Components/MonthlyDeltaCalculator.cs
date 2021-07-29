using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class MonthlyDeltaCalculator : IMonthlyDeltaCalculator {
        private readonly IDataPresenter vDataPresenter;
        private readonly IPostingAggregator vPostingAggregator;

        public MonthlyDeltaCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator) {
            vDataPresenter = dataPresenter;
            vPostingAggregator = postingAggregator;
        }

        public async Task CalculateAndShowMonthlyDeltaAsync(IContainer container, IList<IPosting> allPostings) {
            var minYear = allPostings.Min(p => p.Date.Year);
            var years = Enumerable.Range(minYear, DateTime.Today.Year - minYear + 1);
            var classifications = new List<IPostingClassification>();
            foreach (var year in years) {
                for (var month = 1; month <= 12; month++) {
                    classifications.Add(new PostingClassification {
                        IgnoreCredit = true, Month = month, Year = year,
                        Classification = $"Δ {year}-{month:00}"
                    });
                }
            }

            var errorsAndInfos = new ErrorsAndInfos();
            var monthlyDeltas = vPostingAggregator.AggregatePostings(allPostings, classifications, errorsAndInfos).OrderByDescending(result => result.Key).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var keyLength = monthlyDeltas.Select(a => a.Key.Length).Max();
            foreach (var s in
                from result in monthlyDeltas
                let s = (result.Value >= 0 ? "+" : "") + (int)result.Value
                select $"{result.Key.PadRight(keyLength)}: {s}") {
                await vDataPresenter.WriteLineAsync(DataPresentationOutputType.MonthlyDelta, s);
            }

            var monthlyDeltasList = monthlyDeltas.Select(
                result => new TypeMonthDelta { Type = "Δ", Month = result.Key, Delta = result.Value }
            ).Cast<ITypeMonthDelta>().ToList();
            await container.Resolve<IMonthlyDeltaPresenter>().PresentAsync(monthlyDeltasList);
        }
    }
}
