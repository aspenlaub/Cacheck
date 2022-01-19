namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingClassification {
        bool Credit { get; }
        string Clue { get; }
        string Classification { get; }
        int Month { get; }
        int Year { get; }
        bool Unfair { get; }

        bool IsMonthClassification => Month != 0 && Year != 0;
    }
}
