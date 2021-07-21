using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    public class CacheckIntegrationTestBase {
        protected readonly IContainer Container;
        protected ControllableProcess ControllableProcess;

        public CacheckIntegrationTestBase() {
            Container = new ContainerBuilder().RegisterForCacheckIntegrationTest().Build();
            TestDataGenerator.ResetTestFolder();
        }

        protected async Task<CacheckWindowUnderTest> CreateCacheckWindowUnderTestAsync() {
            var sut = Container.Resolve<CacheckWindowUnderTest>();
            await sut.InitializeAsync();
            await EnsureControllableProcessAsync(sut);

            var process = ControllableProcess;
            var tasks = new List<ControllableProcessTask> {
                sut.CreateResetTask(process)
            };
            await sut.RemotelyProcessTaskListAsync(process, tasks);
            return sut;
        }

        protected async Task EnsureControllableProcessAsync(CacheckWindowUnderTest sut) {
            if (ControllableProcess != null) { return; }

            ControllableProcess = await sut.FindIdleProcessAsync();
        }

        public virtual void Cleanup() {
            TestDataGenerator.RemoveTestFolder();
        }
    }
}
