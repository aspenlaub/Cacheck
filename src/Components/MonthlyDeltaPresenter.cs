using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class MonthlyDeltaPresenter : IMonthlyDeltaPresenter {
        public async Task PresentAsync(IList<ITypeMonthDelta> items) {
            await Task.CompletedTask;
        }
    }
}