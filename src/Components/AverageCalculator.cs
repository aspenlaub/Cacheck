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

public class AverageCalculator : IAverageCalculator {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingAggregator _PostingAggregator;
    private readonly IAggregatedPostingsNetter _AggregatedPostingsNetter;

    public AverageCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator, IAggregatedPostingsNetter aggregatedPostingsNetter) {
        _DataPresenter = dataPresenter;
        _PostingAggregator = postingAggregator;
        _AggregatedPostingsNetter = aggregatedPostingsNetter;
    }

    public async Task CalculateAndShowAverageAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                IList<IInverseClassificationPair> inverseClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();

        var thisYear = allPostings.Max(p => p.Date.Year);
        var halfAYearAgo = allPostings.Max(p => p.Date).AddMonths(-6);
        halfAYearAgo = new DateTime(halfAYearAgo.Year, halfAYearAgo.Month, 1);

        var lastYearsPostings = allPostings.Where(p => p.Date.Year < thisYear).ToList();
        var lastYearsDetailedAggregation = _PostingAggregator.AggregatePostings(lastYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var lastYearsDetailedAggregationList = _AggregatedPostingsNetter.Net(lastYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var thisYearsPostings = allPostings.Where(p => p.Date.Year == thisYear).ToList();
        var thisYearsDetailedAggregation = _PostingAggregator.AggregatePostings(thisYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var thisYearsDetailedAggregationList = _AggregatedPostingsNetter.Net(thisYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var pastHalfYearsPostings = allPostings.Where(p => p.Date > halfAYearAgo).ToList();
        var pastHalfYearsDetailedAggregation = _PostingAggregator.AggregatePostings(pastHalfYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var pastHalfYearsDetailedAggregationList = _AggregatedPostingsNetter.Net(pastHalfYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var detailedAggregation = _PostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var classificationsToKeepEvenIfZero =
            lastYearsDetailedAggregationList
                .Select(x => x.Key.Classification)
                .Union(thisYearsDetailedAggregationList.Select(x => x.Key.Classification))
                .ToList();
        detailedAggregation = _AggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications, classificationsToKeepEvenIfZero);

        var numberOfDistinctMonths = allPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count();
        var numberOfDistinctMonthsLastYear = lastYearsPostings.Any() ? lastYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        var numberOfDistinctMonthsThisYear = thisYearsPostings.Any() ? thisYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        var numberOfDistinctMonthsPastHalfYear = pastHalfYearsPostings.Any() ? pastHalfYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;

        var classificationAverageList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).ToList().Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value / numberOfDistinctMonths,
                SumPastHalfYear = GetOtherSum(result.Key, pastHalfYearsDetailedAggregationList) / numberOfDistinctMonthsPastHalfYear,
                SumThisYear = GetOtherSum(result.Key, thisYearsDetailedAggregationList) / numberOfDistinctMonthsThisYear,
                SumLastYear = GetOtherSum(result.Key, lastYearsDetailedAggregationList) / numberOfDistinctMonthsLastYear
            }
        ).Cast<ICollectionViewSourceEntity>().ToList();
        await _DataPresenter.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(classificationAverageList);
    }

    private static double GetOtherSum(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, double>> otherDetailedAggregation) {
        var results = otherDetailedAggregation.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
        if (!results.Any()) { return 0; }

        return results.Sum();
    }
}