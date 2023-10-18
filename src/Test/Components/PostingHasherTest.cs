using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PostingHasherTest {
    [TestMethod]
    public async Task NoHashesAreEqualForProductionPostings() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null)).Build();
        var postingCollector = container.Resolve<IPostingCollector>();
        var postings = await postingCollector.CollectPostingsAsync(false);
        var hashToPosting = new Dictionary<string, IPosting>();
        var sut = new PostingHasher();
        foreach (var posting in postings) {
            var hash = sut.Hash(posting);
            if (!hashToPosting.ContainsKey(hash)) {
                hashToPosting[hash] = posting;
                continue;
            }

            var otherPosting = hashToPosting[hash];
            Assert.IsFalse(hashToPosting.ContainsKey(hash),
                $"Hash '{hash}' found twice, remarks are {posting.Remark} and {otherPosting.Remark}");
        }
    }

    [TestMethod]
    public void HashDoesNotChange() {
        var sut = new PostingHasher();
        const string expectedHash = "20241018904711014104120229990";
        const string remark = "ARAL AG14.04.2022-99,90";
        var actualHash = sut.Hash(new Posting { Amount = 4711, Date = new DateTime(2024, 10, 18), Remark = remark });
        Assert.AreEqual(expectedHash, actualHash);
    }
}