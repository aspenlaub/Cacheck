namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ICalculationLogger {
    void ClearLogs();
    void Flush();
    void RegisterContribution(string formattedClassificationToString, double amount, IPosting posting);
}