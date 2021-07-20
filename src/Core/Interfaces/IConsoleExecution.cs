using System.Threading.Tasks;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Interfaces {
    public interface IConsoleExecution {
        Task ExecuteAsync(IContainer container, bool isIntegrationTest);
    }
}
