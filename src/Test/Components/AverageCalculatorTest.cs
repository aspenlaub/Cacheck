using System;
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
            Assert.AreEqual(4, items.Count);
            Assert.AreEqual("-", items[0].Type);
            Assert.AreEqual("1510", items[0].Item);
            Assert.AreEqual(1.67, Math.Round(items[0].Sum, 2));
            Assert.AreEqual("-", items[1].Type);
            Assert.AreEqual("1989", items[1].Item);
            Assert.AreEqual(3.33, Math.Round(items[1].Sum, 2));
            Assert.AreEqual("+", items[2].Type);
            Assert.AreEqual("2407", items[2].Item);
            Assert.AreEqual(0.83, Math.Round(items[2].Sum, 2));
            Assert.AreEqual("+", items[3].Type);
            Assert.AreEqual("4711", items[3].Item);
            Assert.AreEqual(2.5, Math.Round(items[3].Sum, 2));
        }
    }
}
