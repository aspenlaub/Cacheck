using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class MonthlyDetailsCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
                IPostingClassificationsMatcher postingClassificationsMatcher) : IMonthlyDetailsCalculator {

    public async Task CalculateAndShowMonthlyDetailsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
        var fairPostings = postingClassificationsMatcher
                           .MatchingPostings(allPostings, postingClassifications, c => c?.Unfair != true)
                           .ToList();
        var monthlyDetailsList = new List<ITypeMonthDetails>();
        var year = fairPostings.Max(p => p.Date.Year);
        var month = fairPostings.Where(p => p.Date.Year == year).Max(p => p.Date.Month);
        for (var i = 0; i < 12; i++) {
            var postings = fairPostings.Where(p => p.Date.Year == year && p.Date.Month == month).ToList();

            var errorsAndInfos = new ErrorsAndInfos();
            var monthDetails = postingAggregator.AggregatePostings(postings, postingClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            foreach (var monthDetail in monthDetails) {
                var listedMonthlyDetail = monthlyDetailsList.FirstOrDefault(
                    m => m.Type == monthDetail.Key.Sign && m.Item == monthDetail.Key.Classification
                );

                if (listedMonthlyDetail == null) {
                    listedMonthlyDetail = new TypeMonthDetails {
                        Type = monthDetail.Key.Sign,
                        Item = monthDetail.Key.Classification
                    };

                    monthlyDetailsList.Add(listedMonthlyDetail);
                }

                switch (month) {
                    case 1:
                        listedMonthlyDetail.January = monthDetail.Value;
                    break;
                    case 2:
                        listedMonthlyDetail.February = monthDetail.Value;
                        break;
                    case 3:
                        listedMonthlyDetail.March = monthDetail.Value;
                        break;
                    case 4:
                        listedMonthlyDetail.April = monthDetail.Value;
                        break;
                    case 5:
                        listedMonthlyDetail.May = monthDetail.Value;
                        break;
                    case 6:
                        listedMonthlyDetail.June = monthDetail.Value;
                        break;
                    case 7:
                        listedMonthlyDetail.July = monthDetail.Value;
                        break;
                    case 8:
                        listedMonthlyDetail.August = monthDetail.Value;
                        break;
                    case 9:
                        listedMonthlyDetail.September = monthDetail.Value;
                        break;
                    case 10:
                        listedMonthlyDetail.October = monthDetail.Value;
                        break;
                    case 11:
                        listedMonthlyDetail.November = monthDetail.Value;
                        break;
                    case 12:
                        listedMonthlyDetail.December = monthDetail.Value;
                        break;
                }
            }

            month--;
            if (month != 0) { continue; }

            month = 12;
            year --;
        }

        await dataPresenter.Handlers.MonthlyDetailsHandler.CollectionChangedAsync(
            monthlyDetailsList.OfType<ICollectionViewSourceEntity>().ToList()
        );
    }
}