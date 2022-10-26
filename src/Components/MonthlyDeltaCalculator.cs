﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class MonthlyDeltaCalculator : IMonthlyDeltaCalculator {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingAggregator _PostingAggregator;
    private readonly IPostingClassificationMatcher _PostingClassificationMatcher;

    public MonthlyDeltaCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator, IPostingClassificationMatcher postingClassificationMatcher) {
        _DataPresenter = dataPresenter;
        _PostingAggregator = postingAggregator;
        _PostingClassificationMatcher = postingClassificationMatcher;
    }

    public async Task CalculateAndShowMonthlyDeltaAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
        var fairPostings = allPostings.Where(p => postingClassifications.FirstOrDefault(c => _PostingClassificationMatcher.DoesPostingMatchClassification(p, c))?.Unfair != true).ToList();
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
        var monthlyDeltas = _PostingAggregator.AggregatePostings(fairPostings, monthsClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var monthlyDeltasList = monthlyDeltas.Select(
            result => new TypeMonthDelta { Type = "Δ", Month = result.Key.Classification.Replace("Δ", "").Trim(), Delta = result.Value }
        ).Cast<ICollectionViewSourceEntity>().ToList();
        await _DataPresenter.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(monthlyDeltasList);
    }
}