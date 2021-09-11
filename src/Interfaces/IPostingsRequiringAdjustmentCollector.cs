using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingsRequiringAdjustmentCollector {
        IEnumerable<IPostingAdjustment> FindNewPostingsRequiringAdjustment(IEnumerable<IPosting> postings, IEnumerable<IPostingAdjustment> adjustments, IEnumerable<ISpecialClue> specialClues);
    }
}
