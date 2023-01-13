using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class FundamentalTransactionsReaderTest {
    private const string TransactionsJsonFileName = "Transactions.json";

    [TestMethod]
    public async Task CanLoadTransactionsIfAvailable() {
        var container = new ContainerBuilder().UsePegh("Cacheck", new DummyCsArgumentPrompter()).Build();
        var resolver = container.Resolve<IFolderResolver>();

        IFundamentalTransactionsReader sut = new FundamentalTransactionsReader(resolver);
        var errorsAndInfos = new ErrorsAndInfos();
        var folder = await sut.FundamentalDumpFolderAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (!folder.Exists()) { return; }
        if (!File.Exists(folder.FullName + @"\" + TransactionsJsonFileName)) { return; }

        var transactions = await sut.LoadTransactionsIfAvailableAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreNotEqual(0, transactions.Count);

        Assert.IsTrue(transactions.Any(t => t.Date.Year == 2022));
        Assert.IsTrue(transactions.Any(t => t.IncomeInEuro > 100));
        Assert.IsTrue(transactions.Any(t => t.TransactionType == TransactionType.Dividend));
        Assert.IsFalse(transactions.Any(t => t.TransactionType == TransactionType.Buy));
        Assert.IsFalse(transactions.Any(t => t.TransactionType == TransactionType.Sell));
    }
}