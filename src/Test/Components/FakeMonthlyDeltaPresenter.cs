using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeMonthlyDeltaPresenter : IMonthlyDeltaPresenter {
        public List<ITypeMonthDelta> Items { get; } = new();

        public async Task PresentAsync(IList<ITypeMonthDelta> items) {
            Items.Clear();
            Items.AddRange(items);
            await Task.CompletedTask;
        }
    }
}
