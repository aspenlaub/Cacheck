using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class FormattedClassification : IFormattedClassification {
    public string Sign { get; init; }
    public string Classification { get; init; }

    public string CombinedClassification { get; init; }

    public override string ToString() {
        return string.IsNullOrEmpty(CombinedClassification) ? $"{Classification} {Sign}" : CombinedClassification;
    }
}