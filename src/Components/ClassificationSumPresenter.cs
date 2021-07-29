using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class ClassificationSumPresenter : IClassificationSumPresenter {
        public async Task PresentAsync(IList<ITypeItemSum> items) {
            await Task.CompletedTask;
        }
    }
}