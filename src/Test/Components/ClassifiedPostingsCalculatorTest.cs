using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class ClassifiedPostingsCalculatorTest : CalculatorTestBase {
    protected IClassifiedPostingsCalculator Sut;

    [TestInitialize]
    public void Initialize() {
        InitializeContainerAndDataPresenter();
        Sut = Container.Resolve<IClassifiedPostingsCalculator>();
    }

    [TestMethod]
    public async Task CanCalculateClassifiedPostings() {
        var postings = new List<IPosting> { TestData.PostingC1, TestData.PostingD1, TestData.PostingC2, TestData.PostingD2 };
        postings = postings.Select(IncreasePostingAmount).ToList();
        var postingClassifications = new List<IPostingClassification> { TestData.PostingClassificationC1, TestData.PostingClassificationD1 };
        await Sut.CalculateAndShowClassifiedPostingsAsync(postings, postingClassifications, new DateTime(2020, 12, 31), 250);

        var items = FakeDataPresenter.ClassifiedPostings;
        Assert.AreEqual(2, items.Count);
        Assert.AreEqual(postings[2].Date, items[0].Date);
        Assert.AreEqual(postings[2].Amount, items[0].Amount);
        Assert.AreEqual(postings[2].Remark, items[0].Remark);
        Assert.AreEqual(postingClassifications[0].Clue, items[0].Clue);
        Assert.AreEqual(postingClassifications[0].Classification, items[0].Classification);
        Assert.AreEqual(postings[1].Date, items[1].Date);
        Assert.AreEqual(postings[1].Amount, items[1].Amount);
        Assert.AreEqual(postings[1].Remark, items[1].Remark);
        Assert.AreEqual(postingClassifications[1].Clue, items[1].Clue);
        Assert.AreEqual(postingClassifications[1].Classification, items[1].Classification);
    }

    private IPosting IncreasePostingAmount(IPosting posting) {
        return new Posting {
            Date = posting.Date,
            Amount = posting.Amount * 24,
            Remark = posting.Remark
        };
    }
}