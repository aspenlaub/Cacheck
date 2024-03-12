using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class MonthlyDeltaCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
                IPostingClassificationsMatcher postingClassificationsMatcher) : IMonthlyDeltaCalculator {

    public async Task CalculateAndShowMonthlyDeltaAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
        var fairPostings = postingClassificationsMatcher
            .MatchingPostings(allPostings, postingClassifications, c => c?.Unfair != true)
            .ToList();
        var minYear = fairPostings.Min(p => p.Date.Year);
        var years = Enumerable.Range(minYear, DateTime.Today.Year - minYear + 1);
        var monthsClassifications = new List<IPostingClassification>();
        foreach (var year in years) {
            for (var month = 1; month <= 12; month++) {
                var classification = postingClassifications.FirstOrDefault(c => c.Month == month && c.Year == year);
                if (classification != null) {
                    monthsClassifications.Add(classification);
                } else {
                    monthsClassifications.Add(new PostingClassification {
                        Month = month,
                        Year = year,
                        Classification = $"Δ {year}-{month:00}"
                    });
                }

            }
        }

        var errorsAndInfos = new ErrorsAndInfos();
        var monthlyDeltas = postingAggregator.AggregatePostings(fairPostings, monthsClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var monthlyDeltasList = monthlyDeltas.Select(
            result => new TypeMonthDelta { Type = "Δ", Month = result.Key.Classification.Replace("Δ", "").Trim(), Delta = result.Value }
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(monthlyDeltasList);
    }
}