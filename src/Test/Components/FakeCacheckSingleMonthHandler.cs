using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;
public class FakeCacheckSingleMonthHandler : ISingleMonthHandler {
    public async Task UpdateSelectableValuesAsync() {
        await Task.CompletedTask;
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        await Task.CompletedTask;
    }

    public async Task UpdateSelectableValuesAsync(IList<IPosting> postings) {
        await Task.CompletedTask;
    }
}
