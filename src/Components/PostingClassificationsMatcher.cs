using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingClassificationsMatcher(IPostingClassificationMatcher postingClassificationMatcher)
                : IPostingClassificationsMatcher {

    public IList<IPostingClassification> MatchingClassifications(IPosting posting, IList<IPostingClassification> postingClassifications) {
        var matchingClassifications = postingClassifications
            .Where(c => c.IsIndividual && postingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
        if (matchingClassifications.Any()) {
            return matchingClassifications;
        }

        matchingClassifications = postingClassifications
            .Where(c => !c.IsUnassigned && postingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
        if (matchingClassifications.Any()) {
            return matchingClassifications;
        }

        return postingClassifications
            .Where(c => c.IsUnassigned && postingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
    }

    public IList<IPosting> MatchingPostings(IList<IPosting> postings, IList<IPostingClassification> postingClassifications,
            Func<IPostingClassification, bool> condition) {
        return postings
           .Where(p => condition(MatchingClassifications(p, postingClassifications).FirstOrDefault()))
           .ToList();
    }

    public IList<IPostingClassification> MatchingClassifications(IList<IPosting> postings,
            IList<IPostingClassification> postingClassifications) {
        var matchingClassifications = postings.ToDictionary(p => p, p => MatchingClassifications(p, postingClassifications));
        return postingClassifications
            .Where(c => postings.Any(p => matchingClassifications[p].Contains(c)))
            .ToList();
    }
}