using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class SpecialClueMatcherTest {
        private readonly PostingTestData TestData = new();

        private static ISpecialClueMatcher Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            Sut = new SpecialClueMatcher();
        }

        [TestMethod]
        public void CanMatchPostingsAndSpecialClue() {
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingC1, TestData.SpecialClue));
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingC2, TestData.SpecialClue));
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingC3, TestData.SpecialClue));
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingD1, TestData.SpecialClue));
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingD2, TestData.SpecialClue));
            Assert.IsFalse(Sut.DoesPostingMatchSpecialClue(TestData.PostingD3, TestData.SpecialClue));
            Assert.IsTrue(Sut.DoesPostingMatchSpecialClue(TestData.SpecialPostingC, TestData.SpecialClue));
            Assert.IsTrue(Sut.DoesPostingMatchSpecialClue(TestData.SpecialPostingD, TestData.SpecialClue));
        }
    }
}