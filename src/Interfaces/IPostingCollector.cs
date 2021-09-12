using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingCollector {
        Task<IList<IPosting>> CollectPostingsAsync(IContainer container, bool isIntegrationTest, IEnumerable<ISpecialClue> specialClues);
        Task<IList<IPostingAdjustment>> GetPostingAdjustmentsAsync(IContainer container, bool isIntegrationTest, IList<IPosting> allPostings, IEnumerable<ISpecialClue> specialClues);
    }
}
