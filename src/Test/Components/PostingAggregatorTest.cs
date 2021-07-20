using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class PostingAggregatorTest {
        protected static IPostingClassification PostingClassification1, PostingClassification2, PostingClassification3, PostingClassification4;
        protected static IPostingClassification PostingClassificationD, PostingClassificationC;
        protected static IPosting Posting1, Posting2, Posting3, Posting4;
        protected const double Amount1 = 10, Amount2 = -20, Amount3 = 30, Amount4 = -40;
        protected static IPostingAggregator Sut;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context) {
            PostingClassification1 = new PostingClassification { Credit = true, Clue = "47", Classification = "4711" };
            PostingClassification2 = new PostingClassification { Credit = true, Clue = "24", Classification = "2407" };
            PostingClassification3 = new PostingClassification { Credit = false, Clue = "15", Classification = "1510" };
            PostingClassification4 = new PostingClassification { Credit = false, Clue = "89", Classification = "1989" };
            PostingClassificationD = new PostingClassification { Credit = false, Clue = "", Classification = "Debit" };
            PostingClassificationC = new PostingClassification { Credit = true, Clue = "", Classification = "Credit" };

            Posting1 = new Posting { Date = DateTime.Today, Amount = Amount1, Remark = "24789"};
            Posting2 = new Posting { Date = DateTime.Today, Amount = Amount2, Remark = "1589" };
            Posting3 = new Posting { Date = DateTime.Today, Amount = Amount3, Remark = "47" };
            Posting4 = new Posting { Date = DateTime.Today, Amount = Amount4, Remark = "89" };

            Sut = new PostingAggregator();
        }

        [TestMethod]
        public void ErrorIfSeveralCluesMatch() {
            var errorsAndInfos = new ErrorsAndInfos();
            Sut.AggregatePostings(new List<IPosting> { Posting1 }, new List<IPostingClassification> { PostingClassification1, PostingClassification2 }, errorsAndInfos);
            Assert.IsTrue(errorsAndInfos.Errors.Any(e => e.Contains("'24'") && e.Contains("'47'")));
        }

        [TestMethod]
        public void CanAggregateSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { Posting1 }, new List<IPostingClassification> { PostingClassification1 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var classification = "(+)" + PostingClassification1.Classification;
            Assert.IsTrue(result.ContainsKey(classification));
            Assert.AreEqual(Posting1.Amount, result[classification]);
        }

        [TestMethod]
        public void DebitCreditIsRespectedWhenAggregatingSinglePosting() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { Posting1 }, new List<IPostingClassification> { PostingClassification1, PostingClassification4 }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(1, result.Count);
            var classification = "(+)" + PostingClassification1.Classification;
            Assert.IsTrue(result.ContainsKey(classification));
            Assert.AreEqual(Posting1.Amount, result[classification]);
        }

        [TestMethod]
        public void CanDoPureDebitCreditAggregation() {
            var errorsAndInfos = new ErrorsAndInfos();
            var result = Sut.AggregatePostings(new List<IPosting> { Posting1, Posting2, Posting3, Posting4 }, new List<IPostingClassification> { PostingClassificationD, PostingClassificationC }, errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.AreEqual(2, result.Count);
            var classification = "(-)" + PostingClassificationD.Classification;
            Assert.IsTrue(result.ContainsKey(classification));
            Assert.AreEqual(-(Posting2.Amount + Posting4.Amount), result[classification]);
            classification = "(+)" + PostingClassificationC.Classification;
            Assert.IsTrue(result.ContainsKey(classification));
            Assert.AreEqual(Posting1.Amount + Posting3.Amount, result[classification]);
        }
    }
}
