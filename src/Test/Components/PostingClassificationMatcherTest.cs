using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingClassificationMatcherTest {
    private readonly PostingTestData _TestData = new();

    private  static IPostingClassificationMatcher Sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context) {
        Sut = new PostingClassificationMatcher();
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
}