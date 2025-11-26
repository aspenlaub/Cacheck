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
        var twoYearsAgo = new DateTime(aYearAgo.Year - 1, aYearAgo.Month, 1);

        var yearPostingsLastYear = allPostings.Where(p => p.Date.Year == thisYear - 1).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedYearAggregationLastYear
            = postingAggregator.AggregatePostings(yearPostingsLastYear, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var detailedYearAggregationListLastYear = aggregatedPostingsNetter.Net(detailedYearAggregationLastYear, inverseClassifications, []).ToList();

        var yearPostingsYearBeforeLast = allPostings.Where(p => p.Date.Year == thisYear - 2).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedYearAggregationYearBeforeLast
            = postingAggregator.AggregatePostings(yearPostingsYearBeforeLast, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var detailedYearAggregationListYearBeforeLast = aggregatedPostingsNetter.Net(detailedYearAggregationYearBeforeLast,
            inverseClassifications, []).ToList();

        IList<IPosting> twoYearsPostingsLastYear = yearPostingsLastYear.Union(yearPostingsYearBeforeLast).ToList();

        var yearPostingsTwoYearsBeforeLast = allPostings.Where(p => p.Date.Year == thisYear - 3).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedYearAggregationTwoYearsBeforeLast
            = postingAggregator.AggregatePostings(yearPostingsTwoYearsBeforeLast, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        IList<IPosting> twoYearsPostingsYearBeforeLast = yearPostingsYearBeforeLast.Union(yearPostingsTwoYearsBeforeLast).ToList();

        var yearPostingsThreeYearsBeforeLast = allPostings.Where(p => p.Date.Year == thisYear - 4).ToList();

        IList<IPosting> twoYearsPostingsTwoYearsBeforeLast = yearPostingsTwoYearsBeforeLast.Union(yearPostingsThreeYearsBeforeLast).ToList();

        var detailedYearAggregationListTwoYearsBeforeLast = aggregatedPostingsNetter.Net(detailedYearAggregationTwoYearsBeforeLast,
            inverseClassifications, []).ToList();

        var thisYearsPostings = allPostings.Where(p => p.Date.Year == thisYear).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> thisYearsDetailedAggregation
            = postingAggregator.AggregatePostings(thisYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var thisYearsDetailedAggregationList = aggregatedPostingsNetter.Net(thisYearsDetailedAggregation, inverseClassifications, []).ToList();

        var twoYearPostingsLastYear = allPostings.Where(p => p.Date.Year == thisYear - 1 || p.Date.Year == thisYear - 2).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedTwoYearAggregationLastYear
            = postingAggregator.AggregatePostings(twoYearPostingsLastYear, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var detailedTwoYearAggregationListLastYear = aggregatedPostingsNetter.Net(detailedTwoYearAggregationLastYear, inverseClassifications, []).ToList();

        var twoYearPostingsYearBeforeLast = allPostings.Where(p => p.Date.Year == thisYear - 2 || p.Date.Year == thisYear - 3).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedTwoYearAggregationYearBeforeLast
            = postingAggregator.AggregatePostings(twoYearPostingsYearBeforeLast, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var detailedTwoYearAggregationListYearBeforeLast = aggregatedPostingsNetter.Net(detailedTwoYearAggregationYearBeforeLast,
                                                                                        inverseClassifications, []).ToList();

        var twoYearPostingsTwoYearsBeforeLast = allPostings.Where(p => p.Date.Year == thisYear - 3 || p.Date.Year == thisYear - 4).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedTwoYearAggregationTwoYearsBeforeLast
            = postingAggregator.AggregatePostings(twoYearPostingsTwoYearsBeforeLast, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var detailedTwoYearAggregationListTwoYearsBeforeLast = aggregatedPostingsNetter.Net(detailedTwoYearAggregationTwoYearsBeforeLast,
            inverseClassifications, []).ToList();

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

        var past24MonthsPostings = allPostings.Where(p => p.Date > twoYearsAgo).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> past24MonthsDetailedAggregation = postingAggregator.AggregatePostings(past24MonthsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var past24MonthsDetailedAggregationList = aggregatedPostingsNetter.Net(past24MonthsDetailedAggregation, inverseClassifications, []).ToList();

        var classificationsToKeepEvenIfZero =
            detailedYearAggregationListLastYear
                .Select(x => x.Key.Classification)
                .Union(thisYearsDetailedAggregationList.Select(x => x.Key.Classification))
                .ToList();
        detailedAggregation = aggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications, classificationsToKeepEvenIfZero);

        int numberOfDistinctMonthsInYearLastYear = yearPostingsLastYear.Any() ? yearPostingsLastYear.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsInYearBeforeLastYear = yearPostingsYearBeforeLast.Any() ? yearPostingsYearBeforeLast.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsInYearTwoYearsBeforeLast = yearPostingsTwoYearsBeforeLast.Any() ? yearPostingsTwoYearsBeforeLast.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsInTwoYearsLastYear = twoYearsPostingsLastYear.Any() ? twoYearsPostingsLastYear.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsInTwoYearsBeforeLastYear = twoYearsPostingsYearBeforeLast.Any() ? twoYearsPostingsYearBeforeLast.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsInTwoYearsTwoYearsBeforeLast = twoYearsPostingsTwoYearsBeforeLast.Any() ? twoYearsPostingsTwoYearsBeforeLast.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastHalfYear = pastHalfYearsPostings.Any() ? pastHalfYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastTwelveMonths = pastTwelveMonthsPostings.Any() ? pastTwelveMonthsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPast24Months = past24MonthsPostings.Any() ? past24MonthsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;

        var classificationAverageList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).ToList().Select(
            result => CreateTypeItemSum(result.Key,
                pastHalfYearsDetailedAggregationList, numberOfDistinctMonthsPastHalfYear,
                pastTwelveMonthsDetailedAggregationList, numberOfDistinctMonthsPastTwelveMonths,
                detailedYearAggregationListLastYear, numberOfDistinctMonthsInYearLastYear,
                detailedYearAggregationListYearBeforeLast, numberOfDistinctMonthsInYearBeforeLastYear,
                detailedYearAggregationListTwoYearsBeforeLast, numberOfDistinctMonthsInYearTwoYearsBeforeLast,
                past24MonthsDetailedAggregationList, numberOfDistinctMonthsPast24Months,
                detailedTwoYearAggregationListLastYear, numberOfDistinctMonthsInTwoYearsLastYear,
                detailedTwoYearAggregationListYearBeforeLast, numberOfDistinctMonthsInTwoYearsBeforeLastYear,
                detailedTwoYearAggregationListTwoYearsBeforeLast, numberOfDistinctMonthsInTwoYearsTwoYearsBeforeLast
            )
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(classificationAverageList);
    }

    private TypeItemSum CreateTypeItemSum(IFormattedClassification classification,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastHalfYearsDetailedAggregationList,
            int numberOfDistinctMonthsPastHalfYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastTwelveMonthsDetailedAggregationList,
            int numberOfDistinctMonthsPastTwelveMonths,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedYearAggregationListLastYear,
            int numberOfDistinctMonthsInYearLastYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedYearAggregationListYearBeforeLast,
            int numberOfDistinctMonthsInYearBeforeLastYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedYearAggregationListTwoYearsBeforeLast,
            int numberOfDistinctMonthsInYearTwoYearsBeforeLast,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> past24MonthsDetailedAggregationList,
            int numberOfDistinctMonthsPast24Months,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedTwoYearAggregationListLastYear,
            int numberOfDistinctMonthsInTwoYearsLastYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedTwoYearAggregationListYearBeforeLast,
            int numberOfDistinctMonthsInTwoYearsBeforeLastYear,
            IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> detailedTwoYearAggregationListTwoYearsBeforeLast,
            int numberOfDistinctMonthsInTwoYearsTwoYearsBeforeLast) {

        double sumPastTwelveMonths = GetOtherSum(classification, pastTwelveMonthsDetailedAggregationList) / numberOfDistinctMonthsPastTwelveMonths;

        double sumPast24Months = GetOtherSum(classification, past24MonthsDetailedAggregationList) / numberOfDistinctMonthsPast24Months;

        return new TypeItemSum { Type = classification.Sign, Item = classification.Classification,
            SumPastHalfYear = GetOtherSum(classification, pastHalfYearsDetailedAggregationList) / numberOfDistinctMonthsPastHalfYear,
            SumPastTwelveMonths = sumPastTwelveMonths,
            SumLastYear = GetOtherSum(classification, detailedYearAggregationListLastYear) / numberOfDistinctMonthsInYearLastYear,
            SumYearBeforeLast = GetOtherSum(classification, detailedYearAggregationListYearBeforeLast) / numberOfDistinctMonthsInYearBeforeLastYear,
            SumTwoYearsBeforeLast = GetOtherSum(classification, detailedYearAggregationListTwoYearsBeforeLast) / numberOfDistinctMonthsInYearTwoYearsBeforeLast,
            SumPast24Months = sumPast24Months,
            SumLastTwoYears = GetOtherSum(classification, detailedTwoYearAggregationListLastYear) / numberOfDistinctMonthsInTwoYearsLastYear,
            TwoYearSumBeforeLastYear = GetOtherSum(classification, detailedTwoYearAggregationListYearBeforeLast) / numberOfDistinctMonthsInTwoYearsBeforeLastYear,
            TwoYearSumTwoYearsBeforeLastYear = GetOtherSum(classification, detailedTwoYearAggregationListTwoYearsBeforeLast) / numberOfDistinctMonthsInTwoYearsTwoYearsBeforeLast
        };
    }

    private static double GetOtherSum(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> otherDetailedAggregation) {
        var results = otherDetailedAggregation.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
        return results.Select(x => x.Sum).Sum();
    }
}