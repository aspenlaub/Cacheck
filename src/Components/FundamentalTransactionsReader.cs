using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class FundamentalTransactionsReader(IFolderResolver folderResolver) : IFundamentalTransactionsReader {

    private const string _transactionsJsonFileName = "Transactions.json";

    public async Task<IFolder> FundamentalDumpFolderAsync(IErrorsAndInfos errorsAndInfos) {
        IFolder folder = await folderResolver.ResolveAsync(@"$(MainUserFolder)\Fundamental\Production\Dump", errorsAndInfos);
        return folder;
    }

    public async Task<IList<Transaction>> LoadTransactionsIfAvailableAsync(IErrorsAndInfos errorsAndInfos) {
        IFolder folder = await FundamentalDumpFolderAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors() || !folder.Exists()) { return new List<Transaction>(); }

        string fileName = folder.FullName + @"\" + _transactionsJsonFileName;
        if (!File.Exists(fileName)) { return new List<Transaction>(); }

        await using var stream = new FileStream(fileName, FileMode.Open);
        List<Transaction> transactions = await JsonSerializer.DeserializeAsync<List<Transaction>>(stream) ?? [];
        transactions = transactions.Where(t => t.TransactionType != TransactionType.Buy && t.TransactionType != TransactionType.Sell).ToList();
        return transactions;
    }
}