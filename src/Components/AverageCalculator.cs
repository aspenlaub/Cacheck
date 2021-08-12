using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class AverageCalculator : IAverageCalculator {
        private readonly IDataPresenter vDataPresenter;
        private readonly IPostingAggregator vPostingAggregator;

        public AverageCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator) {
            vDataPresenter = dataPresenter;
            vPostingAggregator = postingAggregator;
        }

        public async Task CalculateAndShowAverageAsync(IContainer container, IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
            var errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = vPostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos).OrderBy(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var thisYear = allPostings.Max(p => p.Date.Year);
            var lastYearsPostings = allPostings.Where(p => p.Date.Year < thisYear).ToList();
            var lastYearsDetailedAggregation = vPostingAggregator.AggregatePostings(lastYearsPostings, postingClassifications, errorsAndInfos).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var thisYearsPostings = allPostings.Where(p => p.Date.Year == thisYear).ToList();
            var thisYearsDetailedAggregation = vPostingAggregator.AggregatePostings(thisYearsPostings, postingClassifications, errorsAndInfos).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await vDataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            var numberOfDistinctMonths = allPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count();
            var numberOfDistinctMonthsLastYear = lastYearsPostings.Any() ? lastYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;
            var numberOfDistinctMonthsThisYear = thisYearsPostings.Any() ? thisYearsPostings.Select(p => p.Date.Month * 100 + p.Date.Year).Distinct().Count() : 1;

            var classificationAverageList = detailedAggregation.Select(
                result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value / numberOfDistinctMonths,
                    SumThisYear = GetOtherSum(result.Key, thisYearsDetailedAggregation) / numberOfDistinctMonthsThisYear,
                    SumLastYear = GetOtherSum(result.Key, lastYearsDetailedAggregation) / numberOfDistinctMonthsLastYear
                }
            ).Cast<ICollectionViewSourceEntity>().ToList();
            await vDataPresenter.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(classificationAverageList);
        }

        private double GetOtherSum(IFormattedClassification formattedClassification, IEnumerable<KeyValuePair<IFormattedClassification, double>> otherDetailedAggregation) {
            var results = otherDetailedAggregation.Where(f => f.Key.CombinedClassification == formattedClassification.CombinedClassification).Select(f => f.Value).ToList();
            if (!results.Any()) { return 0; }

            return results.Sum();
        }
    }
}
