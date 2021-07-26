using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class PostingClassificationMatcherTest {
        private readonly PostingTestData vTestData = new();

        protected static IPostingClassificationMatcher Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            Sut = new PostingClassificationMatcher();
        }

        [TestMethod]
        public void CanMatchPostingsAndClassifications() {
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationC1));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationC2));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationD1));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationD2));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationD));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationC));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationJuly));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationAugust));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC1, vTestData.PostingClassificationSeptember));

            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationC1));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationC2));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationD1));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationD2));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationD));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationC));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationJuly));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationAugust));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD1, vTestData.PostingClassificationSeptember));

            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationC1));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationC2));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationD1));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationD2));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationD));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationC));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationJuly));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationAugust));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingC2, vTestData.PostingClassificationSeptember));

            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationC1));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationC2));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationD1));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationD2));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationD));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationC));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationJuly));
            Assert.IsTrue(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationAugust));
            Assert.IsFalse(Sut.DoesPostingMatchClassification(vTestData.PostingD2, vTestData.PostingClassificationSeptember));
        }
    }
}
