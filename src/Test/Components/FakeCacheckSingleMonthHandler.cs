using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;
public class FakeCacheckSingleMonthHandler : ISimpleSelectorHandler {
    public async Task UpdateSelectableValuesAsync() {
        await Task.CompletedTask;
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        await Task.CompletedTask;
    }
}
