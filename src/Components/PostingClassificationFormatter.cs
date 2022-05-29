using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingClassificationFormatter : IPostingClassificationFormatter {
    public IFormattedClassification Format(IPostingClassification classification) {
        return new FormattedClassification {
            Sign = classification.IsMonthClassification ? "" : classification.Credit ? "+" : "-",
            Classification = classification.Classification,
            CombinedClassification  = (classification.IsMonthClassification ? "" : classification.Credit ? "(+) " : "(-) ") + classification.Classification
        };
    }
}