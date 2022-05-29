namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IFormattedClassification {
    string Sign { get; }
    string Classification { get; }

    string CombinedClassification { get; }
}