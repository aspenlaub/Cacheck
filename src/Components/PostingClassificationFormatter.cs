using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingClassificationFormatter : IPostingClassificationFormatter {
        public string Format(IPostingClassification classification) {
            return (classification.IgnoreCredit ? "" : classification.Credit ? "(+) " : "(-) ") + classification.Classification;
        }
    }
}
