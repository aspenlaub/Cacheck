using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingClassificationFormatter : IPostingClassificationFormatter {
        public IFormattedClassification Format(IPostingClassification classification) {
            return new FormattedClassification {
                Sign = classification.IgnoreCredit ? "" : classification.Credit ? "+" : "-",
                Classification = classification.Classification,
                CombinedClassification  = (classification.IgnoreCredit ? "" : classification.Credit ? "(+) " : "(-) ") + classification.Classification
            };
        }
    }
}
