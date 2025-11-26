using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class MonthlyDeltaCalculatorTest : CalculatorTestBase {
    protected IMonthlyDeltaCalculator Sut;

    [TestInitialize]
    public void Initialize() {
        InitializeContainerAndDataPresenter();
        Sut = Container.Resolve<IMonthlyDeltaCalculator>();
    }

    [TestMethod]
    public async Task CanCalculateMonthlyDelta() {
        await Sut.CalculateAndShowMonthlyDeltaAsync(
            [
                TestData.PostingC1, TestData.PostingD1, TestData.PostingC2,
                TestData.PostingD2
            ],
            [ TestData.PostingClassificationJuly, TestData.PostingClassificationAugust, TestData.PostingClassificationSeptember ]
        );

        List<ITypeMonthDelta> items = FakeDataPresenter.MonthlyDeltas;
        Assert.HasCount(2, items);
        Assert.AreEqual("Δ", items[0].Type);
        Assert.AreEqual("2021-08", items[0].Month);
        Assert.AreEqual(-40, items[0].Delta);
        Assert.AreEqual("Δ", items[1].Type);
        Assert.AreEqual("2021-07", items[1].Month);
        Assert.AreEqual(20, items[1].Delta);
    }
}