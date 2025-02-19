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

    private static IPostingAggregator _sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) {
        IContainer container = new ContainerBuilder().UsePegh("Cacheck", new DummyCsArgumentPrompter()).Build();
        var classificationsMatcher = new PostingClassificationsMatcher(
            new PostingClassificationMatcher(new PostingHasher())
        );
        _sut = new PostingAggregator(new PostingClassificationFormatter(), classificationsMatcher,
            new FormattedClassificationComparer(), new CalculationLogger(container.Resolve<ILogConfiguration>()));
    }

    [TestMethod]
    public void ErrorIfSeveralCluesMatch() {
        var errorsAndInfos = new ErrorsAndInfos();
        _sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1, _TestData.PostingClassificationC2 }, errorsAndInfos);
        Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("'(+) 2407'") && e.Contains("'(+) 4711'")));
    }

    [TestMethod]
    public void CanAggregateSinglePosting() {
        var errorsAndInfos = new ErrorsAndInfos();
        IDictionary<IFormattedClassification, IAggregatedPosting> result = _sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1 }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(1, result.Count);
        IFormattedClassification key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC1.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount, result[key].Sum);
    }

    [TestMethod]
    public void DebitCreditIsRespectedWhenAggregatingSinglePosting() {
        var errorsAndInfos = new ErrorsAndInfos();
        IDictionary<IFormattedClassification, IAggregatedPosting> result = _sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1 }, new List<IPostingClassification> { _TestData.PostingClassificationC1, _TestData.PostingClassificationD2 }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(1, result.Count);
        IFormattedClassification key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC1.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount, result[key].Sum);
    }

    [TestMethod]
    public void CanDoPureDebitCreditAggregation() {
        var errorsAndInfos = new ErrorsAndInfos();
        IDictionary<IFormattedClassification, IAggregatedPosting> result = _sut.AggregatePostings(new List<IPosting> { _TestData.PostingC1, _TestData.PostingD1, _TestData.PostingC2, _TestData.PostingD2 },
                                                                                     new List<IPostingClassification> { _TestData.PostingClassificationD, _TestData.PostingClassificationC }, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(2, result.Count);
        IFormattedClassification key = result.Keys.FirstOrDefault(x => x.Sign == "-" && x.Classification == _TestData.PostingClassificationD.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(-(_TestData.PostingD1.Amount + _TestData.PostingD2.Amount), result[key].Sum);
        key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == _TestData.PostingClassificationC.Classification);
        Assert.IsNotNull(key);
        Assert.AreEqual(_TestData.PostingC1.Amount + _TestData.PostingC2.Amount, result[key].Sum);
    }
}