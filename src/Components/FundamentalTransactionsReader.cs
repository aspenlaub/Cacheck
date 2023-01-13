using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class FundamentalTransactionsReader : IFundamentalTransactionsReader {
    private const string TransactionsJsonFileName = "Transactions.json";

    private readonly IFolderResolver _FolderResolver;

    public FundamentalTransactionsReader(IFolderResolver folderResolver) {
        _FolderResolver = folderResolver;
    }
    public async Task<IFolder> FundamentalDumpFolderAsync(IErrorsAndInfos errorsAndInfos) {
        var folder = await _FolderResolver.ResolveAsync(@"$(MainUserFolder)\Fundamental\Production\Dump", errorsAndInfos);
        return folder;
    }

    public async Task<IList<Transaction>> LoadTransactionsIfAvailableAsync(IErrorsAndInfos errorsAndInfos) {
        var folder = await FundamentalDumpFolderAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors() || !folder.Exists()) { return new List<Transaction>(); }

        var fileName = folder.FullName + @"\" + TransactionsJsonFileName;
        if (!File.Exists(fileName)) { return new List<Transaction>(); }

        var stream = new FileStream(fileName, FileMode.Open);
        var transactions = await JsonSerializer.DeserializeAsync<List<Transaction>>(stream) ?? new List<Transaction>();
        transactions = transactions.Where(t => t.TransactionType != TransactionType.Buy && t.TransactionType != TransactionType.Sell).ToList();
        return transactions;
    }
}