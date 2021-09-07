using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class MonthlyDeltaCalculator : IMonthlyDeltaCalculator {
        private readonly IDataPresenter DataPresenter;
        private readonly IPostingAggregator PostingAggregator;
        private readonly IPostingClassificationMatcher PostingClassificationMatcher;

        public MonthlyDeltaCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator, IPostingClassificationMatcher postingClassificationMatcher) {
            DataPresenter = dataPresenter;
            PostingAggregator = postingAggregator;
            PostingClassificationMatcher = postingClassificationMatcher;
        }

        public async Task CalculateAndShowMonthlyDeltaAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
            var minYear = allPostings.Min(p => p.Date.Year);
            var years = Enumerable.Range(minYear, DateTime.Today.Year - minYear + 1);
            var monthsClassifications = new List<IPostingClassification>();
            foreach (var year in years) {
                for (var month = 1; month <= 12; month++) {
                    monthsClassifications.Add(new PostingClassification {
                        IgnoreCredit = true, Month = month, Year = year,
                        Classification = $"Δ {year}-{month:00}"
                    });
                }
            }

            var errorsAndInfos = new ErrorsAndInfos();
            var monthlyDeltas = PostingAggregator.AggregatePostings(allPostings, monthsClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var fairPostings = allPostings.Where(p => postingClassifications.All(c => !c.ExcludeFromFairCalculation || !PostingClassificationMatcher.DoesPostingMatchClassification(p, c))).ToList();
            var fairMonthlyDeltas = PostingAggregator.AggregatePostings(fairPostings, monthsClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var monthlyDeltasList = monthlyDeltas.Select(
                result => new TypeMonthDelta { Type = "Δ", Month = result.Key.Classification.Replace("Δ", "").Trim(), Delta = result.Value, FairDelta = GetFairMonthlyDelta(result.Key, fairMonthlyDeltas) }
            ).Cast<ICollectionViewSourceEntity>().ToList();
            await DataPresenter.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(monthlyDeltasList);
        }

        private static double GetFairMonthlyDelta(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, double>> fairMonthlyDeltas) {
            var results = fairMonthlyDeltas.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
            if (!results.Any()) { return 0; }

            return results.Sum();
        }
    }
}
