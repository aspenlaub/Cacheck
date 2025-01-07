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

public class MonthlyDetailsCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
            IPostingClassificationsMatcher postingClassificationsMatcher) : IMonthlyDetailsCalculator {

    public async Task CalculateAndShowMonthlyDetailsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                    double minimumAmount, int fromDay, int toDay) {

        var fairPostings = postingClassificationsMatcher
                           .MatchingPostings(allPostings, postingClassifications, c => c?.Unfair != true)
                           .Where(c => Math.Abs(c.Amount) >= minimumAmount)
                           .ToList();
        var monthlyDetailsList = new List<ITypeMonthDetails>();
        int year = fairPostings.Max(p => p.Date.Year);
        int month = fairPostings.Where(p => p.Date.Year == year).Max(p => p.Date.Month);
        for (int i = 0; i < 12; i++) {
            var postings = fairPostings.Where(p => p.Date.Year == year
                && p.Date.Month == month && p.Date.Day >= fromDay && p.Date.Day <= toDay).ToList();

            var errorsAndInfos = new ErrorsAndInfos();
            var monthDetails = postingAggregator.AggregatePostings(postings, postingClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            foreach (KeyValuePair<IFormattedClassification, double> monthDetail in monthDetails) {
                ITypeMonthDetails listedMonthlyDetail = monthlyDetailsList.FirstOrDefault(
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
                    default:
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