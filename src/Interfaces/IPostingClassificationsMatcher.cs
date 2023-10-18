using System;
using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingClassificationsMatcher {
    IList<IPostingClassification> MatchingClassifications(IPosting posting,
        IList<IPostingClassification> postingClassifications);
    IList<IPostingClassification> MatchingClassifications(IList<IPosting> postings,
        IList<IPostingClassification> postingClassifications);
    IList<IPosting> MatchingPostings(IList<IPosting> postings,
        IList<IPostingClassification> postingClassifications,
        Func<IPostingClassification, bool> condition);
}