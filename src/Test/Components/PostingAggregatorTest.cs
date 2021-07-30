using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class PostingAggregatorTest {
        private readonly PostingTestData vTestData = new();

        protected static IPostingAggregator Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            Sut = new PostingAggregator(new PostingClassificationFormatter(), new PostingClassificationMatcher(), new FormattedClassificationComparer());
        }

        [TestMethod]
        public void ErrorIfSeveralCluesMatch() {
            var errorsAndInfos = new ErrorsAndInfos();
            Sut.AggregatePostings(new List<IPosting> { vTestData.PostingC1 }, new List<IPostingClassification> { vTestData.PostingClassificationC1, vTestData.PostingClassificationC2 }, errorsAndInfos);
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("'24'") && e.Contains("'47'")));
        }

        [TestMethod]
        public void CanAggregateSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { vTestData.PostingC1 }, new List<IPostingClassification> { vTestData.PostingClassificationC1 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == vTestData.PostingClassificationC1.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(vTestData.PostingC1.Amount, result[key]);
        }

        [TestMethod]
        public void DebitCreditIsRespectedWhenAggregatingSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { vTestData.PostingC1 }, new List<IPostingClassification> { vTestData.PostingClassificationC1, vTestData.PostingClassificationD2 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == vTestData.PostingClassificationC1.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(vTestData.PostingC1.Amount, result[key]);
        }

        [TestMethod]
        public void CanDoPureDebitCreditAggregation() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { vTestData.PostingC1, vTestData.PostingD1, vTestData.PostingC2, vTestData.PostingD2 }, new List<IPostingClassification> { vTestData.PostingClassificationD, vTestData.PostingClassificationC }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(2, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "-" && x.Classification == vTestData.PostingClassificationD.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(-(vTestData.PostingD1.Amount + vTestData.PostingD2.Amount), result[key]);
            key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == vTestData.PostingClassificationC.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(vTestData.PostingC1.Amount + vTestData.PostingC2.Amount, result[key]);
        }
    }
}
