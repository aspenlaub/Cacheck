using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class SpecialClueMatcher : ISpecialClueMatcher {
        public bool DoesPostingMatchSpecialClue(IPosting posting, ISpecialClue classification) {
            return string.IsNullOrEmpty(classification.Clue) || posting.Remark.Contains(classification.Clue, StringComparison.OrdinalIgnoreCase);
        }
    }
}