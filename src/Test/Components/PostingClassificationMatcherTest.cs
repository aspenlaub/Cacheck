using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingClassificationMatcherTest {
    private readonly PostingTestData _TestData = new();

    private static IPostingHasher _postingHasher;
    private static IIndividualPostingClassificationConverter _individualPostingClassificationConverter;
    private static IPostingClassificationMatcher _sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) {
        _postingHasher = new PostingHasher();
        _individualPostingClassificationConverter = new IndividualPostingClassificationConverter();
        _sut = new PostingClassificationMatcher(_postingHasher);
    }

    [TestMethod]
    public void CanMatchPostingsAndClassifications() {
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC1));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC2));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD1));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD2));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationJuly));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationAugust));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationSeptember));

        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC1));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC2));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD1));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD2));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationJuly));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationAugust));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationSeptember));

        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC1));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC2));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD1));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD2));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationJuly));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationAugust));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationSeptember));

        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC1));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC2));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD1));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD2));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationJuly));
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationAugust));
        Assert.IsFalse(_sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationSeptember));
    }

    [TestMethod]
    public void CanMatchPostingsAndIndividualClassifications() {
        IPostingClassification classification = CreateIndividualClassification(_TestData.PostingC1, _TestData.PostingClassificationC1);
        Assert.IsTrue(_sut.DoesPostingMatchClassification(_TestData.PostingC1, classification));
        var otherPostings = new List<IPosting> {
            _TestData.PostingC2, _TestData.PostingC3,
            _TestData.PostingD1, _TestData.PostingD2, _TestData.PostingD3
        };
        foreach(IPosting otherPosting in otherPostings) {
            Assert.IsFalse(_sut.DoesPostingMatchClassification(otherPosting, classification));
        }
    }

    private IPostingClassification CreateIndividualClassification(IPosting posting, IPostingClassification classification) {
        var individualPostingClassification = new IndividualPostingClassification {
            Classification = classification.Classification,
            Credit = classification.Credit,
            PostingHash = _postingHasher.Hash(posting)
        };
        return _individualPostingClassificationConverter.Convert(individualPostingClassification);
    }
}