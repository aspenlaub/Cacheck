using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class MonthlyDetailsCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
                IPostingClassificationsMatcher postingClassificationsMatcher) : IMonthlyDetailsCalculator {

    public async Task CalculateAndShowMonthlyDetailsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
        var monthlyDeltasList = new List<ITypeMonthDetails>().OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.MonthlyDetailsHandler.CollectionChangedAsync(monthlyDeltasList);
    }
}