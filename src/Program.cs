using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    internal class Program {
        private static async Task Main(string[] args) {
            var container = new ContainerBuilder().UseCacheckAndPegh(new DummyCsArgumentPrompter()).Build();
            var execution = container.Resolve<IConsoleExecution>();
            var isIntegrationTest = args.Any(a => a == "/UnitTest");
            await execution.ExecuteAsync(isIntegrationTest);
        }
    }
}
