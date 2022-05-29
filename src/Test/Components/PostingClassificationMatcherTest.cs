using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingClassificationMatcherTest {
    private readonly PostingTestData TestData = new();

    private  static IPostingClassificationMatcher Sut;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context) {
        Sut = new PostingClassificationMatcher();
    }

    [TestMethod]
    public void CanMatchPostingsAndClassifications() {
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationC1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationD1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationD2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationD));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC1, TestData.PostingClassificationSeptember));

        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationC2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationD1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationD2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationD));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD1, TestData.PostingClassificationSeptember));

        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationD1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationD2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationD));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationC));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationJuly));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingC2, TestData.PostingClassificationSeptember));

        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationC1));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationC2));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationD1));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationD2));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationD));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationC));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationJuly));
        Assert.IsTrue(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationAugust));
        Assert.IsFalse(Sut.DoesPostingMatchClassification(TestData.PostingD2, TestData.PostingClassificationSeptember));
    }
}