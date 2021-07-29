using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IListPresenter<T> where T : ICollectionViewSourceEntity {
        Task PresentAsync(IList<T> items);
    }
}
