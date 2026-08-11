using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingHasherTest {
    [TestMethod]
    public async Task NoHashesAreEqualForProductionPostings() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null)).Build();
        IPostingCollector postingCollector = container.Resolve<IPostingCollector>();
        IList<IPosting> postings = await postingCollector.CollectPostingsAsync(false);
        if (postings.AreAllPostingsPreClassified()) {
            Assert.Inconclusive("Test requires postings in the source folder");
        }
        if (postings.Count < 24) {
            Assert.Inconclusive("Not enough production postings");
        }
        var hashToPosting = new Dictionary<string, IPosting>();
        var sut = new PostingHasher();
        foreach (IPosting posting in postings) {
            string hash = sut.Hash(posting);
            if (!hashToPosting.TryGetValue(hash, out IPosting otherPosting)) {
                hashToPosting[hash] = posting;
                continue;
            }

            // TODO: Find out what goes wrong with capital income and remove the following lines
            if (posting.Date == otherPosting.Date
                    && Math.Abs(posting.Amount - otherPosting.Amount) < 0.001
                    && posting.Remark == otherPosting.Remark
                    && posting.OriginalRemark == otherPosting.OriginalRemark) {
                continue;
            }

            Assert.IsFalse(hashToPosting.ContainsKey(hash),
                $"Hash '{hash}' found twice, for {posting.ToString()} and {otherPosting.ToString()}");
        }
    }

    [TestMethod]
    public void HashDoesNotChange() {
        var sut = new PostingHasher();
        const string expectedHash = "20241018904711014104120229990";
        const string remark = "ARAL AG14.04.2022-99,90";
        string actualHash = sut.Hash(new Posting { Amount = 4711, Date = new DateTime(2024, 10, 18), Remark = remark });
        Assert.AreEqual(expectedHash, actualHash);
    }
}