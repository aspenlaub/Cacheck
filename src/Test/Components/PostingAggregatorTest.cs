using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingAggregatorTest {
    private readonly PostingTestData _TestData = new();

    private static IPostingAggregator Sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context) {
        var container = new ContainerBuilder().UsePegh("Cacheck", new DummyCsArgumentPrompter()).Build();
        Sut = new PostingAggregator(new PostingClassificationFormatter(), new PostingClassificationMatcher(), new FormattedClassificationComparer(), new CalculationLogger(container.Resolve<ILogConfiguration>()));
    }

    [TestMethod]
    public void ErrorIfSeveralCluesMatch() {
        var errorsAndInfos = new ErrorsAndInfos();
        Sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1, _TestData.PostingClassificationC2 }, errorsAndInfos);
        Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("'(+) 2407'") && e.Contains("'(+) 4711'")));
    }

    [TestMethod]
    public void CanAggregateSinglePosting() {
        var errorsAndInfos = new ErrorsAndInfos();
        var result = Sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1 }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(1, result.Count);
        var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC1.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount, result[key]);
    }

    [TestMethod]
    public void DebitCreditIsRespectedWhenAggregatingSinglePosting() {
        var errorsAndInfos = new ErrorsAndInfos();
        var result = Sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1, _TestData.PostingClassificationD2 }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(1, result.Count);
        var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC1.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount, result[key]);
    }

    [TestMethod]
    public void CanDoPureDebitCreditAggregation() {
        var errorsAndInfos = new ErrorsAndInfos();
        var result = Sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1, _TestData.PostingD1, _TestData.PostingC2, _TestData.PostingD2 },
            new List<IPostingClassification> { _TestData.PostingClassificationD, _TestData.PostingClassificationC }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(2, result.Count);
        var key = result.Keys.FirstOrDefault(x => x.Sign == "-" && x.Classification == _TestData.PostingClassificationD.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(-(_TestData.PostingD1.Amount + _TestData.PostingD2.Amount), result[key]);
        key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount + _TestData.PostingC2.Amount, result[key]);
    }
}