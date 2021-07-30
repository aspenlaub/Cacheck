using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeClassificationSumPresenter : IClassificationSumPresenter {
        public List<ITypeItemSum> Items { get; } = new();

        public async Task PresentAsync(IList<ITypeItemSum> items) {
            Items.Clear();
            Items.AddRange(items);
            await Task.CompletedTask;
        }
    }
}