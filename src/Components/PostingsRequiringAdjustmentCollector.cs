using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingsRequiringAdjustmentCollector : IPostingsRequiringAdjustmentCollector {
        private readonly ISpecialClueMatcher SpecialClueMatcher;

        public PostingsRequiringAdjustmentCollector(ISpecialClueMatcher specialClueMatcher) {
            SpecialClueMatcher = specialClueMatcher;
        }

        public IEnumerable<IPostingAdjustment> FindNewPostingsRequiringAdjustment(IEnumerable<IPosting> postings, IEnumerable<IPostingAdjustment> adjustments, IEnumerable<ISpecialClue> specialClues) {
            return postings
                .Select(p => CreatePostingAdjustment(p, specialClues))
                .Where(a => a != null)
                .Where(a => adjustments.All(a2 => a.Date != a2.Date || Math.Abs(a.Amount - a2.Amount) > 0.005 || a.Clue != a2.Clue))
                .ToList();
        }

        private IPostingAdjustment CreatePostingAdjustment(IPosting posting, IEnumerable<ISpecialClue> specialClues) {
            var clue = specialClues.FirstOrDefault(c => SpecialClueMatcher.DoesPostingMatchSpecialClue(posting, c));
            return clue == null
                ? null
                : new PostingAdjustment { Date = posting.Date, Amount = posting.Amount, AdjustedAmount = posting.Amount, Clue = clue.Clue };
        }
    }
}
