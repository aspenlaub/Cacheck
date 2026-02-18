using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IDataCollector {
    Task CollectAndShowAsync();
    Task CollectAndShowMonthlyDetailsAsync();
    Task<List<IPostingClassification>> CollectPostingClassificationsAsync(IErrorsAndInfos errorsAndInfos);
    Task<List<IInverseClassificationPair>> CollectInverseClassifications(ErrorsAndInfos errorsAndInfos);
}