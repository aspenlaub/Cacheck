using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        await Sut.CalculateAndShowSummaryAsync(new List<IPosting> { TestData.PostingC2, TestData.PostingD2, TestData.PostingC3, TestData.PostingD3 },
            new List<IPostingClassification> { TestData.PostingClassificationC1, TestData.PostingClassificationC2, TestData.PostingClassificationD1, TestData.PostingClassificationD2 },
            new List<IInverseClassificationPair>());

        var items = FakeDataPresenter.OverallSums;
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual("+", items[0].Type);
        Assert.AreEqual("Credit", items[0].Item);
        Assert.AreEqual(40, items[0].Sum);
        Assert.AreEqual("-", items[1].Type);
        Assert.AreEqual("Debit", items[1].Item);
        Assert.AreEqual(60, items[1].Sum);

        items = FakeDataPresenter.ClassificationSums;
        Assert.AreEqual(4, items.Count);
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