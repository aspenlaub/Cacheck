namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingClassification {
        bool IgnoreCredit { get; }
        bool Credit { get; }
        string Clue { get; }
        string Classification { get; }
        bool ExcludeFromFairCalculation { get; }
        int Month { get; }
        int Year { get; }
    }
}
