using System.Collections.Generic;
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
    private const string _transactionsJsonFileName = "Transactions.json";

    [TestMethod]
    public async Task CanLoadTransactionsIfAvailable() {
        IContainer container = new ContainerBuilder().UsePegh("Cacheck", new DummyCsArgumentPrompter()).Build();
        IFolderResolver resolver = container.Resolve<IFolderResolver>();

        IFundamentalTransactionsReader sut = new FundamentalTransactionsReader(resolver);
        var errorsAndInfos = new ErrorsAndInfos();
        IFolder folder = await sut.FundamentalDumpFolderAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (!folder.Exists()) { return; }
        if (!File.Exists(folder.FullName + @"\" + _transactionsJsonFileName)) { return; }

        IList<Transaction> transactions = await sut.LoadTransactionsIfAvailableAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreNotEqual(0, transactions.Count);

        Assert.IsTrue(transactions.Any(t => t.Date.Year == 2022));
        Assert.IsTrue(transactions.Any(t => t.IncomeInEuro > 100));
        Assert.IsTrue(transactions.Any(t => t.TransactionType == TransactionType.Dividend));
        Assert.IsFalse(transactions.Any(t => t.TransactionType == TransactionType.Buy));
        Assert.IsFalse(transactions.Any(t => t.TransactionType == TransactionType.Sell));
    }
}