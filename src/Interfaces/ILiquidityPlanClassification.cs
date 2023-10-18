namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ILiquidityPlanClassification {
    string Classification { get; init; }
    int Percentage { get; init; }
    double Target { get; init; }
}