using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class MonthlyDeltaCalculatorTest {
        private readonly PostingTestData vTestData = new();

        protected IContainer Container;
        protected FakeDataPresenter FakeDataPresenter;
        protected IMonthlyDeltaCalculator Sut;

        [TestInitialize]
        public void Initialize() {
            Container = new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null).Result.Build();
            FakeDataPresenter = new FakeDataPresenter();
            Sut = new MonthlyDeltaCalculator(FakeDataPresenter, Container.Resolve<IPostingAggregator>());
        }

        [TestMethod]
        public async Task CanCalculateMonthlyDelta() {
            await Sut.CalculateAndShowMonthlyDeltaAsync(Container, new List<IPosting> { vTestData.PostingC1, vTestData.PostingD1, vTestData.PostingC2, vTestData.PostingD2 });
            Assert.IsTrue(FakeDataPresenter.Output.ContainsKey(DataPresentationOutputType.MonthlyDelta));
            var output = FakeDataPresenter.Output[DataPresentationOutputType.MonthlyDelta];
            Assert.AreEqual(2, output.Count);
            Assert.AreEqual("Δ 2021-08: -40", output[0]);
            Assert.AreEqual("Δ 2021-07: +20", output[1]);
        }
    }
}
