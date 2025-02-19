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

public class SingleMonthDeltasCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
        IPostingClassificationsMatcher postingClassificationsMatcher) : ISingleMonthDeltasCalculator {
    public async Task CalculateAndShowSingleMonthDeltasAsync(IList<IPosting> allTimePostings,
            IList<IPostingClassification> postingClassifications, int month, int currentYear) {
        var singleMonthDeltasList = new List<ITypeSingleMonthDelta>();
        if (month <= 0 || currentYear <= 0) {
            await dataPresenter.Handlers.SingleMonthDeltasHandler.CollectionChangedAsync(
                singleMonthDeltasList.OfType<ICollectionViewSourceEntity>().ToList()
            );
            return;
        }

        for (int yearOffset = 0; yearOffset < 3; yearOffset++) {
            int year = currentYear - yearOffset;
            var postings = allTimePostings.Where(p => p.Date.Month == month && p.Date.Year == year).ToList();
            var fairPostings = postingClassificationsMatcher
               .MatchingPostings(postings, postingClassifications, c => c?.Unfair != true)
               .ToList();
            var errorsAndInfos = new ErrorsAndInfos();
            var monthDeltas = postingAggregator.AggregatePostings(fairPostings, postingClassifications, errorsAndInfos).OrderByDescending(result => result.Key.CombinedClassification).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return;
            }

            foreach (KeyValuePair<IFormattedClassification, IAggregatedPosting> monthDelta in monthDeltas) {
                ITypeSingleMonthDelta listedSingleMonthDelta = singleMonthDeltasList.FirstOrDefault(
                    m => m.Type == monthDelta.Key.Sign && m.Item == monthDelta.Key.Classification
                );

                if (listedSingleMonthDelta == null) {
                    listedSingleMonthDelta = new TypeSingleMonthDelta { Type = monthDelta.Key.Sign, Item = monthDelta.Key.Classification };

                    singleMonthDeltasList.Add(listedSingleMonthDelta);
                }

                switch (yearOffset) {
                    case 0: listedSingleMonthDelta.CurrentYear = monthDelta.Value.Sum; break;
                    case 1: listedSingleMonthDelta.YearBefore = monthDelta.Value.Sum; break;
                    case 2: listedSingleMonthDelta.TwoYearsBefore = monthDelta.Value.Sum; break;
                    default: throw new NotImplementedException();
                }
            }
        }
        await dataPresenter.Handlers.SingleMonthDeltasHandler.CollectionChangedAsync(
            singleMonthDeltasList.OfType<ICollectionViewSourceEntity>().ToList()
        );
    }
}
