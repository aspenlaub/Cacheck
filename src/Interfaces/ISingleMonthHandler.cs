using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
public interface ISingleMonthHandler : ISimpleSelectorHandler {
    Task UpdateSelectableValuesAsync(IList<IPosting> postings);
}
