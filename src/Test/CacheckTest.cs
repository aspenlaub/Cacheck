using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test {
    [TestClass]
    public class CacheckTest {
        [TestMethod]
        public void CacheckTestMethod() {
            var container = new ContainerBuilder().UseCacheckAndPegh(new DummyCsArgumentPrompter()).Build();
            Assert.IsNotNull(container);
        }
    }
}
