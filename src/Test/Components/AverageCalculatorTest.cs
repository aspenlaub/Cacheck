using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

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
        await Sut.CalculateAndShowAverageAsync(
           [ TestData.PostingC2, TestData.PostingD2, TestData.PostingC3, TestData.PostingD3 ],
           [
               TestData.PostingClassificationC1, TestData.PostingClassificationC2, TestData.PostingClassificationD1,
               TestData.PostingClassificationD2
           ],
           []);

        List<ITypeItemSum> items = FakeDataPresenter.ClassificationAverages;
        VerifyResult(items[0], "-", "1510", 10, 10, 0);
        VerifyResult(items[1], "-", "1989", 20, 20, 0);
        VerifyResult(items[2], "+", "2407", 5, 5, 0);
        VerifyResult(items[3], "+", "4711", 15, 15, 0);
    }

    protected static void VerifyResult(ITypeItemSum item, string expectedType, string expectedItem,
            double expectedSumPastHalfYear, double expectedSumPastTwelveMonths, double expectedSumAsOfLastYear) {
        Assert.AreEqual(expectedType, item.Type);
        Assert.AreEqual(expectedItem, item.Item);
        Assert.AreEqual(expectedSumPastHalfYear, item.SumPastHalfYear);
        Assert.AreEqual(expectedSumPastTwelveMonths, item.SumPastTwelveMonths);
        Assert.AreEqual(expectedSumAsOfLastYear, item.SumLastYear);
    }
}