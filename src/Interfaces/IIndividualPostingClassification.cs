namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IIndividualPostingClassification {
    bool Credit { get; }
    string PostingHash { get; }
    string Classification { get; }
    bool Ineliminable { get; }
}