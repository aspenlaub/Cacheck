using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingsRequiringAdjustmentCollector {
        IEnumerable<IPostingAdjustment> FindNewPostingsRequiringAdjustment(IEnumerable<IPosting> postings, IList<IPostingAdjustment> adjustments, IEnumerable<ISpecialClue> specialClues);
        void AssignReferenceToAdjustments(IEnumerable<IPosting> postings, IList<IPostingAdjustment> adjustments, IEnumerable<ISpecialClue> specialClues);
    }
}
