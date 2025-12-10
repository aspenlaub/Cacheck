namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPreClassifiedPosting : IPosting {
    string Classification { get; }
    bool IsIndividual { get; }
    bool Ineliminable { get; }
    bool Unfair { get; }
}
