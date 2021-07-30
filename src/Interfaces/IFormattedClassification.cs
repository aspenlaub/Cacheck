namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IFormattedClassification {
        string Sign { get; set; }
        string Classification { get; set; }

        string CombinedClassification { get; set; }
    }
}
