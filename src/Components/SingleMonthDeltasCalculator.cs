using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SingleMonthDeltasCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
        IPostingClassificationsMatcher postingClassificationsMatcher) : ISingleMonthDeltasCalculator {
    public async Task CalculateAndShowSingleMonthDeltasAsync(IList<IPosting> allPostings,
            IList<IPostingClassification> postingClassifications, int month) {
        var singleMonthDeltasList = new List<ITypeSingleMonthDelta>();
        if (month > 0) {
            ITypeSingleMonthDelta singleMonthDelta = new TypeSingleMonthDelta {
                Guid = Guid.NewGuid().ToString(),
                Item = "Not implemented yet",
                Type = "",
                CurrentYear = 0,
                YearBefore = 0,
                TwoYearsBefore = 0
            };
            singleMonthDeltasList.Add(singleMonthDelta);
        }
        await dataPresenter.Handlers.SingleMonthDeltasHandler.CollectionChangedAsync(
            singleMonthDeltasList.OfType<ICollectionViewSourceEntity>().ToList()
        );
    }
}
