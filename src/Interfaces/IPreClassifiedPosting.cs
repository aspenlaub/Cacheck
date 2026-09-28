namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPreClassifiedPosting : IPosting {
    string Classification { get; }
    string Sha1 { get; set; }
    bool IsIndividual { get; }
    bool Ineliminable { get; }
    bool Unfair { get; }
}
