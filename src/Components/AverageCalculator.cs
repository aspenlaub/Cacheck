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

public class AverageCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
                IAggregatedPostingsNetter aggregatedPostingsNetter) : IAverageCalculator {

    public async Task CalculateAndShowAverageAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                                                   IList<IInverseClassificationPair> inverseClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();

        int thisYear = allPostings.Max(p => p.Date.Year);
        DateTime halfAYearAgo = allPostings.Max(p => p.Date).AddMonths(-6);
        halfAYearAgo = new DateTime(halfAYearAgo.Year, halfAYearAgo.Month, 1);
        DateTime aYearAgo = allPostings.Max(p => p.Date).AddMonths(-12);
        aYearAgo = new DateTime(aYearAgo.Year, aYearAgo.Month, 1);

        var lastYearsPostings = allPostings.Where(p => p.Date.Year == thisYear - 1).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> lastYearsDetailedAggregation
            = postingAggregator.AggregatePostings(lastYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var lastYearsDetailedAggregationList = aggregatedPostingsNetter.Net(lastYearsDetailedAggregation, inverseClassifications, []).ToList();

        var yearBeforeLastsPostings = allPostings.Where(p => p.Date.Year == thisYear - 2).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> yearBeforeLastsDetailedAggregation
            = postingAggregator.AggregatePostings(yearBeforeLastsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var yearBeforeLastsDetailedAggregationList = aggregatedPostingsNetter.Net(yearBeforeLastsDetailedAggregation, inverseClassifications, []).ToList();

        var twoYearsBeforeLastsPostings = allPostings.Where(p => p.Date.Year == thisYear - 3).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> twoYearsBeforeLastsDetailedAggregation
            = postingAggregator.AggregatePostings(twoYearsBeforeLastsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var twoYearsBeforeLastsDetailedAggregationList = aggregatedPostingsNetter.Net(twoYearsBeforeLastsDetailedAggregation, inverseClassifications, []).ToList();

        var thisYearsPostings = allPostings.Where(p => p.Date.Year == thisYear).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> thisYearsDetailedAggregation = postingAggregator.AggregatePostings(thisYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var thisYearsDetailedAggregationList = aggregatedPostingsNetter.Net(thisYearsDetailedAggregation, inverseClassifications, []).ToList();

        var pastHalfYearsPostings = allPostings.Where(p => p.Date > halfAYearAgo).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> pastHalfYearsDetailedAggregation = postingAggregator.AggregatePostings(pastHalfYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var pastHalfYearsDetailedAggregationList = aggregatedPostingsNetter.Net(pastHalfYearsDetailedAggregation, inverseClassifications, []).ToList();

        var pastTwelveMonthsPostings = allPostings.Where(p => p.Date > aYearAgo).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> pastTwelveMonthsDetailedAggregation = postingAggregator.AggregatePostings(pastTwelveMonthsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var pastTwelveMonthsDetailedAggregationList = aggregatedPostingsNetter.Net(pastTwelveMonthsDetailedAggregation, inverseClassifications, []).ToList();

        IDictionary<IFormattedClassification, IAggregatedPosting> detailedAggregation = postingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var classificationsToKeepEvenIfZero =
            lastYearsDetailedAggregationList
                .Select(x => x.Key.Classification)
                .Union(thisYearsDetailedAggregationList.Select(x => x.Key.Classification))
                .ToList();
        detailedAggregation = aggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications, classificationsToKeepEvenIfZero);

        int numberOfDistinctMonthsLastYear = lastYearsPostings.Any() ? lastYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsYearBeforeLast = yearBeforeLastsPostings.Any() ? yearBeforeLastsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsTwoYearsBeforeLast = twoYearsBeforeLastsPostings.Any() ? twoYearsBeforeLastsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastHalfYear = pastHalfYearsPostings.Any() ? pastHalfYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastTwelveMonths = pastTwelveMonthsPostings.Any() ? pastTwelveMonthsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;

        var classificationAverageList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).ToList().Select(
            result => CreateTypeItemSum(result.Key,
                pastHalfYearsDetailedAggregationList, numberOfDistinctMonthsPastHalfYear,
                pastTwelveMonthsDetailedAggregationList, numberOfDistinctMonthsPastTwelveMonths,
                lastYearsDetailedAggregationList, numberOfDistinctMonthsLastYear,
                yearBeforeLastsDetailedAggregationList, numberOfDistinctMonthsYearBeforeLast,
                twoYearsBeforeLastsDetailedAggregationList, numberOfDistinctMonthsTwoYearsBeforeLast
            )
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(classificationAverageList);
    }

    private TypeItemSum CreateTypeItemSum(IFormattedClassification classification,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastHalfYearsDetailedAggregationList,
            int numberOfDistinctMonthsPastHalfYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastTwelveMonthsDetailedAggregationList,
            int numberOfDistinctMonthsPastTwelveMonths,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> lastYearsDetailedAggregationList,
            int numberOfDistinctMonthsLastYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> yearBeforeLastsDetailedAggregationList,
            int numberOfDistinctMonthsYearBeforeLast,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> twoYearsBeforeLastsDetailedAggregationList,
            int numberOfDistinctMonthsTwoYearsBeforeLast) {
        double sumPastTwelveMonths = GetOtherSum(classification, pastTwelveMonthsDetailedAggregationList) / numberOfDistinctMonthsPastTwelveMonths;
        return new TypeItemSum { Type = classification.Sign, Item = classification.Classification,
            SumPastHalfYear = GetOtherSum(classification, pastHalfYearsDetailedAggregationList) / numberOfDistinctMonthsPastHalfYear,
            SumPastTwelveMonths = sumPastTwelveMonths,
            SumLastYear = GetOtherSum(classification, lastYearsDetailedAggregationList) / numberOfDistinctMonthsLastYear,
            SumYearBeforeLast = GetOtherSum(classification, yearBeforeLastsDetailedAggregationList) / numberOfDistinctMonthsYearBeforeLast,
            SumTwoYearsBeforeLast = GetOtherSum(classification, twoYearsBeforeLastsDetailedAggregationList) / numberOfDistinctMonthsTwoYearsBeforeLast,
            SumLastTwoYears = 0,
            TwoYearSumBeforeLastYear = 0,
            TwoYearSumTwoYearsBeforeLastYear = 0
        };
    }

    private static double GetOtherSum(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> otherDetailedAggregation) {
        var results = otherDetailedAggregation.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
        return results.Select(x => x.Sum).Sum();
    }
}