using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    [TestClass]
    public class CacheckWindowTest : CacheckIntegrationTestBase {
        [TestCleanup]
        public override void Cleanup() {
            base.Cleanup();
        }

        [TestMethod]
        public async Task CanOpenCacheck() {
            using (await CreateCacheckWindowUnderTestAsync()) {}
        }
    }
}
