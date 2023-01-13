using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class FundamentalTransactionsReaderTest {
    [TestMethod]
    public void CanLoadTransactionsIfAvailable() {
        IFundamentalTransactionsReader sut = new FundamentalTransactionsReader();
        var transactions = sut.LoadTransactionsIfAvailable();
        Assert.AreEqual(0, transactions.Count);
    }
}