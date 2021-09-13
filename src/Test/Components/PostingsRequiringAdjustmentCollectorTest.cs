using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class PostingsRequiringAdjustmentCollectorTest {
        [TestMethod]
        public void CanFindNewPostingsRequiringAdjustment() {
            var sut = new PostingsRequiringAdjustmentCollector(new SpecialClueMatcher());
            var postings = new List<IPosting>();
            var adjustments = new List<IPostingAdjustment>();
            var clues = new List<ISpecialClue>();
            Assert.IsFalse(sut.FindNewPostingsRequiringAdjustment(postings, adjustments, clues).Any());
            postings.Add(CreatePosting("Remark4711", new DateTime(2021, 9, 11), 4711));
            postings.Add(CreatePosting("Remark4712", new DateTime(2021, 9, 11), 4712));
            Assert.IsFalse(sut.FindNewPostingsRequiringAdjustment(postings, adjustments, clues).Any());
            clues.Add(CreateClue("4713"));
            Assert.IsFalse(sut.FindNewPostingsRequiringAdjustment(postings, adjustments, clues).Any());
            clues.Add(CreateClue("4711"));
            var newAdjustments = sut.FindNewPostingsRequiringAdjustment(postings, adjustments, clues).ToList();
            Assert.AreEqual(1, newAdjustments.Count);
            Assert.AreEqual(postings[0].Date, newAdjustments[0].Date);
            Assert.AreEqual(postings[0].Amount, newAdjustments[0].Amount);
            Assert.AreEqual(postings[0].Amount, newAdjustments[0].AdjustedAmount);
            Assert.AreEqual(clues[1].Clue, newAdjustments[0].Clue);
            adjustments.AddRange(newAdjustments);
            Assert.IsFalse(sut.FindNewPostingsRequiringAdjustment(postings, adjustments, clues).Any());
            sut.AssignReferenceToAdjustments(postings, adjustments, clues);
            Assert.AreEqual(postings[0].Guid, adjustments[0].Reference);
        }

        private IPosting CreatePosting(string remark, DateTime date, double amount) {
            return new Posting { Date = date, Amount = amount, Remark = remark };
        }

        private ISpecialClue CreateClue(string clue) {
            return new SpecialClue { Clue = clue };
        }
    }
}
