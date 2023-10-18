using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IIndividualPostingClassificationsSource {
    Task<IEnumerable<IIndividualPostingClassification>> GetAsync(IErrorsAndInfos errorsAndInfos);
    Task RemoveAsync(IndividualPostingClassification individualPostingClassification);
    Task AddAsync(IndividualPostingClassification individualPostingClassification, IErrorsAndInfos errorsAndInfos);
}