using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class IndividualPostingClassificationConverter : IIndividualPostingClassificationConverter {
    public IPostingClassification Convert(IIndividualPostingClassification individualPostingClassification) {
        return new PostingClassification {
            Credit = individualPostingClassification.Credit,
            Classification = individualPostingClassification.Classification,
            Clue = individualPostingClassification.PostingHash,
            IsIndividual = true,
            PostingHash = individualPostingClassification.PostingHash,
            Ineliminable = individualPostingClassification.Ineliminable
        };
    }
}