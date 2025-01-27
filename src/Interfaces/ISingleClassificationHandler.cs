using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ISingleClassificationHandler : ISimpleSelectorHandler {
    Task UpdateSelectableValuesAsync(bool areWeCollecting);
    Task UpdateSelectableValuesAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
        IList<IInverseClassificationPair> inverseClassifications, bool areWeCollecting);
}