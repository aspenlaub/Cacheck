using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IConsoleExecution {
        Task ExecuteAsync(bool isIntegrationTest);
    }
}
