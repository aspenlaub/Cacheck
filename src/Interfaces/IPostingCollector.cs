using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingCollector {
    Task<IList<IPosting>> CollectPostingsAsync(bool isIntegrationTest);
}