using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IIndividualPostingClassificationsSource {
    Task<IEnumerable<IIndividualPostingClassification>> GetAsync(IErrorsAndInfos errorsAndInfos);
    Task RemoveAsync(IndividualPostingClassification individualPostingClassification);
    Task AddOrUpdateAsync(IndividualPostingClassification individualPostingClassification, IErrorsAndInfos errorsAndInfos);
}