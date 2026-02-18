using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IClassifiedPostingsImporter {
    Task<IList<IPosting>> ImportClassifiedPostingsAsync(string importFileFullName, IErrorsAndInfos errorsAndInfos);
}
