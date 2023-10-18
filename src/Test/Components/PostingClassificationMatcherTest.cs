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

    private static IPostingHasher PostingHasher;
    private static IIndividualPostingClassificationConverter IndividualPostingClassificationConverter;
    private static IPostingClassificationMatcher Sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext _) {
        PostingHasher = new PostingHasher();
        IndividualPostingClassificationConverter = new IndividualPostingClassificationConverter();
        Sut = new PostingClassificationMatcher(PostingHasher);
    }

    [TestMethod]
    public void CanMatchPostingsAndClassifications() {
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationD));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC1, _TestData.PostingClassificationSeptember));

        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationD));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD1, _TestData.PostingClassificationSeptember));

        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationD));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingC2, _TestData.PostingClassificationSeptember));

        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationD));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationC));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationJuly));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(_TestData.PostingD2, _TestData.PostingClassificationSeptember));
    }

    [TestMethod]
    public void CanMatchPostingsAndIndividualClassifications() {
        var classification = CreateIndividualClassification(_TestData.PostingC1, _TestData.PostingClassificationC1);
        Assert.IsTrue(Sut.DoesPostingMatchClassification(_TestData.PostingC1, classification));
        var otherPostings = new List<IPosting> {
            _TestData.PostingC2, _TestData.PostingC3,
            _TestData.PostingD1, _TestData.PostingD2, _TestData.PostingD3
        };
        foreach(var otherPosting in otherPostings) {
            Assert.IsFalse(Sut.DoesPostingMatchClassification(otherPosting, classification));
        }
    }

    private IPostingClassification CreateIndividualClassification(IPosting posting, IPostingClassification classification) {
        var individualPostingClassification = new IndividualPostingClassification {
            Classification = classification.Classification,
            Credit = classification.Credit,
            PostingHash = PostingHasher.Hash(posting)
        };
        return IndividualPostingClassificationConverter.Convert(individualPostingClassification);
    }
}