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

        public MonthlyDeltaCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator) {
            DataPresenter = dataPresenter;
            PostingAggregator = postingAggregator;
        }

        public async Task CalculateAndShowMonthlyDeltaAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                IList<ISpecialClue> specialClues, IList<IPostingAdjustment> postingAdjustments) {
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
            var monthlyDeltas = PostingAggregator.AggregatePostings(allPostings, monthsClassifications, specialClues, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            foreach (var postingAdjustment in postingAdjustments.Where(p => Math.Abs(p.Amount - p.AdjustedAmount) > 0.005)) {
                if (string.IsNullOrEmpty(postingAdjustment.Reference)) {
                    errorsAndInfos.Errors.Add($"Adjustment on {postingAdjustment.Date} with amount {postingAdjustment.Amount} does not have a reference");
                    await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                    return;
                }

                var postingAndIndex = allPostings
                    .Select((p, i) => (p, i))
                    .FirstOrDefault(x => x.p.Guid == postingAdjustment.Reference);
                var posting = postingAndIndex.p;
                if (postingAndIndex.p == null) {
                    errorsAndInfos.Errors.Add($"Adjustment reference {postingAdjustment.Reference} not found");
                    await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                    return;
                }

                if (Math.Abs(posting.Amount - postingAdjustment.Amount) > 0.005) {
                    errorsAndInfos.Errors.Add($"Adjustment reference {postingAdjustment.Reference} suggests amount {posting.Amount}, registered is {postingAdjustment.Amount}");
                    await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                    return;
                }

                allPostings[postingAndIndex.i] = new Posting {
                    Date = posting.Date, Remark = posting.Remark, Amount = postingAdjustment.AdjustedAmount
                };
            }
            var fairMonthlyDeltas = PostingAggregator.AggregatePostings(allPostings, monthsClassifications, specialClues, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
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
