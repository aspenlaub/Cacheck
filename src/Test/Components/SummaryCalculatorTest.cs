using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: DoNotParallelize]
namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class SummaryCalculatorTest : CalculatorTestBase {
    protected ISummaryCalculator Sut;

    [TestInitialize]
    public void Initialize() {
        InitializeContainerAndDataPresenter();
        Sut = Container.Resolve<ISummaryCalculator>();
    }

    [TestMethod]
    public async Task CanCalculateSummary() {
        bool success = await Sut.CalculateAndShowSummaryAsync(
            [
                TestData.PostingC2, TestData.PostingD2, TestData.PostingC3,
                TestData.PostingD3
            ],
            [
                TestData.PostingClassificationC1, TestData.PostingClassificationC2, TestData.PostingClassificationD1,
                TestData.PostingClassificationD2
            ],
            []);

        Assert.IsTrue(success);
        List<ITypeItemSum> items = FakeDataPresenter.OverallSums;
        Assert.HasCount(2, items);
        Assert.AreEqual("+", items[0].Type);
        Assert.AreEqual("Credit", items[0].Item);
        Assert.AreEqual(40, items[0].Sum);
        Assert.AreEqual("-", items[1].Type);
        Assert.AreEqual("Debit", items[1].Item);
        Assert.AreEqual(60, items[1].Sum);

        items = FakeDataPresenter.ClassificationSums;
        Assert.HasCount(4, items);
        Assert.AreEqual("-", items[0].Type);
        Assert.AreEqual("1510", items[0].Item);
        Assert.AreEqual(20, items[0].Sum);
        Assert.AreEqual("-", items[1].Type);
        Assert.AreEqual("1989", items[1].Item);
        Assert.AreEqual(40, items[1].Sum);
        Assert.AreEqual("+", items[2].Type);
        Assert.AreEqual("2407", items[2].Item);
        Assert.AreEqual(10, items[2].Sum);
        Assert.AreEqual("+", items[3].Type);
        Assert.AreEqual("4711", items[3].Item);
        Assert.AreEqual(30, items[3].Sum);
    }
}