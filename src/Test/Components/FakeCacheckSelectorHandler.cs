using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

public class FakeCacheckSelectorHandler : ISingleClassificationHandler {
    public async Task UpdateSelectableValuesAsync() {
        await Task.CompletedTask;
    }

    public async Task UpdateSelectableValuesAsync(bool areWeCollecting) {
        await Task.CompletedTask;
    }

    public async Task UpdateSelectableValuesAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
            IList<IInverseClassificationPair> inverseClassifications, bool areWeCollecting) {
        await Task.CompletedTask;
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        await Task.CompletedTask;
    }
}