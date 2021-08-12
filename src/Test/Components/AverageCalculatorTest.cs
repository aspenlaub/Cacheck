using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class AverageCalculatorTest : CalculatorTestBase {
        protected IAverageCalculator Sut;

        [TestInitialize]
        public void Initialize() {
            InitializeContainerAndDataPresenter();
            Sut = Container.Resolve<IAverageCalculator>();
        }

        [TestMethod]
        public async Task CanCalculateAverage() {
            await Sut.CalculateAndShowAverageAsync(Container,
                new List<IPosting> { TestData.PostingC2, TestData.PostingD2, TestData.PostingC3, TestData.PostingD3 },
                new List<IPostingClassification> { TestData.PostingClassificationC1, TestData.PostingClassificationC2, TestData.PostingClassificationD1, TestData.PostingClassificationD2 });

            var items = FakeDataPresenter.ClassificationAverages;
            VerifyResult(items[0], "-", "1510", 10, 0);
            VerifyResult(items[1], "-", "1989", 20, 0);
            VerifyResult(items[2], "+", "2407", 5, 0);
            VerifyResult(items[3], "+", "4711", 15, 0);
        }

        protected static void VerifyResult(ITypeItemSum item, string expectedType, string expectedItem, double expectedSum, double expectedSumAsOfLastYear) {
            Assert.AreEqual(expectedType, item.Type);
            Assert.AreEqual(expectedItem, item.Item);
            Assert.AreEqual(expectedSum, item.Sum);
            Assert.AreEqual(expectedSum, item.SumThisYear);
            Assert.AreEqual(expectedSumAsOfLastYear, item.SumLastYear);
        }
    }
}
