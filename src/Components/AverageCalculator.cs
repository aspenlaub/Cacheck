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
                IAggregatedPostingsNetter aggregatedPostingsNetter,
                ILiquidityPlanCalculator liquidityPlanCalculator,
                IReservationsCalculator reservationsCalculator) : IAverageCalculator {

    public async Task CalculateAndShowAverageAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                                                   IList<IInverseClassificationPair> inverseClassifications, IList<ILiquidityPlanClassification> liquidityPlanClassifications,
                                                   IList<IIrregularDebitClassification> irregularDebitClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();

        int thisYear = allPostings.Max(p => p.Date.Year);
        DateTime halfAYearAgo = allPostings.Max(p => p.Date).AddMonths(-6);
        halfAYearAgo = new DateTime(halfAYearAgo.Year, halfAYearAgo.Month, 1);
        DateTime aYearAgo = allPostings.Max(p => p.Date).AddMonths(-12);
        aYearAgo = new DateTime(aYearAgo.Year, aYearAgo.Month, 1);

        var lastYearsPostings = allPostings.Where(p => p.Date.Year < thisYear).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> lastYearsDetailedAggregation = postingAggregator.AggregatePostings(lastYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var lastYearsDetailedAggregationList = aggregatedPostingsNetter.Net(lastYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var thisYearsPostings = allPostings.Where(p => p.Date.Year == thisYear).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> thisYearsDetailedAggregation = postingAggregator.AggregatePostings(thisYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var thisYearsDetailedAggregationList = aggregatedPostingsNetter.Net(thisYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var pastHalfYearsPostings = allPostings.Where(p => p.Date > halfAYearAgo).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> pastHalfYearsDetailedAggregation = postingAggregator.AggregatePostings(pastHalfYearsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var pastHalfYearsDetailedAggregationList = aggregatedPostingsNetter.Net(pastHalfYearsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

        var pastTwelveMonthsPostings = allPostings.Where(p => p.Date > aYearAgo).ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> pastTwelveMonthsDetailedAggregation = postingAggregator.AggregatePostings(pastTwelveMonthsPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var pastTwelveMonthsDetailedAggregationList = aggregatedPostingsNetter.Net(pastTwelveMonthsDetailedAggregation, inverseClassifications, new List<string>()).ToList();

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

        int numberOfDistinctMonths = allPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count();
        int numberOfDistinctMonthsLastYear = lastYearsPostings.Any() ? lastYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsThisYear = thisYearsPostings.Any() ? thisYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastHalfYear = pastHalfYearsPostings.Any() ? pastHalfYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
        int numberOfDistinctMonthsPastTwelveMonths = pastTwelveMonthsPostings.Any() ? pastTwelveMonthsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;

        double liquidityPlanSum = 0, reservationsSum = 0;
        var classificationAverageList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).ToList().Select(
            result => CreateTypeItemSum(result.Key, result.Value.Sum, numberOfDistinctMonths, pastHalfYearsDetailedAggregationList,
            numberOfDistinctMonthsPastHalfYear, pastTwelveMonthsDetailedAggregationList, numberOfDistinctMonthsPastTwelveMonths,
            thisYearsDetailedAggregationList, numberOfDistinctMonthsThisYear, lastYearsDetailedAggregationList,
            numberOfDistinctMonthsLastYear, liquidityPlanClassifications, irregularDebitClassifications,
            ref liquidityPlanSum, ref reservationsSum)
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(classificationAverageList);

        await dataPresenter.OnSumsChanged(liquidityPlanSum, reservationsSum);
    }

    private TypeItemSum CreateTypeItemSum(IFormattedClassification classification, double sum,
            int numberOfDistinctMonths, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastHalfYearsDetailedAggregationList,
            int numberOfDistinctMonthsPastHalfYear, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> pastTwelveMonthsDetailedAggregationList,
            int numberOfDistinctMonthsPastTwelveMonths, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> thisYearsDetailedAggregationList,
            int numberOfDistinctMonthsThisYear, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> lastYearsDetailedAggregationList,
            int numberOfDistinctMonthsLastYear, IList<ILiquidityPlanClassification> liquidityPlanClassifications,
            IList<IIrregularDebitClassification> irregularDebitClassifications,
            ref double liquidityPlanSum, ref double reservationsSum) {
        double sumPastTwelveMonths = GetOtherSum(classification, pastTwelveMonthsDetailedAggregationList) / numberOfDistinctMonthsPastTwelveMonths;
        double liquidityPlanContribution = liquidityPlanCalculator.Calculate(classification, sumPastTwelveMonths, liquidityPlanClassifications);
        liquidityPlanSum += liquidityPlanContribution;
        double reservationsContribution = reservationsCalculator.Calculate(classification, sumPastTwelveMonths, irregularDebitClassifications);
        reservationsSum += reservationsContribution;
        return new TypeItemSum { Type = classification.Sign, Item = classification.Classification, Sum = sum / numberOfDistinctMonths,
            SumPastHalfYear = GetOtherSum(classification, pastHalfYearsDetailedAggregationList) / numberOfDistinctMonthsPastHalfYear,
            SumPastTwelveMonths = sumPastTwelveMonths,
            LiquidityPlan = liquidityPlanContribution,
            Reservation = reservationsContribution,
            SumThisYear = GetOtherSum(classification, thisYearsDetailedAggregationList) / numberOfDistinctMonthsThisYear,
            SumLastYear = GetOtherSum(classification, lastYearsDetailedAggregationList) / numberOfDistinctMonthsLastYear
        };
    }

    private static double GetOtherSum(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, IAggregatedPosting>> otherDetailedAggregation) {
        var results = otherDetailedAggregation.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
        if (!results.Any()) { return 0; }

        return results.Select(x => x.Sum).Sum();
    }
}