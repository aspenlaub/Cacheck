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
        private readonly PostingTestData TestData = new();

        private static IPostingAggregator Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            Sut = new PostingAggregator(new PostingClassificationFormatter(), new PostingClassificationMatcher(), new FormattedClassificationComparer());
        }

        [TestMethod]
        public void ErrorIfSeveralCluesMatch() {
            var errorsAndInfos = new ErrorsAndInfos();
            Sut.AggregatePostings(new List<IPosting> { TestData.PostingC1 }, new List<IPostingClassification> { TestData.PostingClassificationC1, TestData.PostingClassificationC2 }, errorsAndInfos);
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("'(+) 2407'") && e.Contains("'(+) 4711'")));
        }

        [TestMethod]
        public void CanAggregateSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { TestData.PostingC1 }, new List<IPostingClassification> { TestData.PostingClassificationC1 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == TestData.PostingClassificationC1.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(TestData.PostingC1.Amount, result[key]);
        }

        [TestMethod]
        public void DebitCreditIsRespectedWhenAggregatingSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { TestData.PostingC1 }, new List<IPostingClassification> { TestData.PostingClassificationC1, TestData.PostingClassificationD2 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == TestData.PostingClassificationC1.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(TestData.PostingC1.Amount, result[key]);
        }

        [TestMethod]
        public void CanDoPureDebitCreditAggregation() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { TestData.PostingC1, TestData.PostingD1, TestData.PostingC2, TestData.PostingD2 },
                new List<IPostingClassification> { TestData.PostingClassificationD, TestData.PostingClassificationC }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(2, result.Count);
            var key = result.Keys.FirstOrDefault(x => x.Sign == "-" && x.Classification == TestData.PostingClassificationD.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(-(TestData.PostingD1.Amount + TestData.PostingD2.Amount), result[key]);
            key = result.Keys.FirstOrDefault(x => x.Sign == "+" && x.Classification == TestData.PostingClassificationC.Classification);
            Assert.IsNotNull(key);
            Assert.AreEqual(TestData.PostingC1.Amount + TestData.PostingC2.Amount, result[key]);
        }
    }
}
