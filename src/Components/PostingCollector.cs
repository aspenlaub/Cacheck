using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingCollector(IDataPresenter dataPresenter, ISecretRepository secretRepository,
                IFolderResolver folderResolver, ISourceFileReader sourceFileReader,
                IFundamentalTransactionsReader fundamentalTransactionsReader,
                ITransactionIntoPostingsConverter transactionIntoPostingConverter,
                IClassifiedPostingsImporter importer) : IPostingCollector {

    public async Task<IList<IPosting>> CollectPostingsAsync(bool isIntegrationTest) {
        IFolder sourceFolder = await GetSourceFolderAsync(isIntegrationTest);
        if (sourceFolder == null) { return []; }

        var errorsAndInfos = new ErrorsAndInfos();
        List<IPosting> allPostings = await LoadPostingsFromSourceFolder(sourceFolder, errorsAndInfos);
        if (allPostings.Count == 0 && !isIntegrationTest) {
            string importFileFullName = await PreClassifiedPostingsSettings.ClassifiedPostingsFileFullNameAsync(folderResolver, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return []; }

            allPostings = [.. await importer.ImportClassifiedPostingsAsync(importFileFullName, errorsAndInfos)];
            return errorsAndInfos.AnyErrors() ? [] : allPostings;
        }

        IList<Transaction> transactions = await fundamentalTransactionsReader.LoadTransactionsIfAvailableAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return allPostings;
        }

        if (allPostings.Count == 0) {
            return allPostings;
        }

        DateTime minDate = allPostings.Min(p => p.Date);
        DateTime maxDate = allPostings.Max(p => p.Date);
        transactions = [.. transactions.Where(t
            => t.Date.Year >= minDate.Year && (t.Date.Year < maxDate.Year || t.Date.Year == maxDate.Year && t.Date.Month <= maxDate.Month)
        )];

        allPostings.AddRange(transactions.SelectMany(transactionIntoPostingConverter.Convert));

        return allPostings;
    }

    private async Task<List<IPosting>> LoadPostingsFromSourceFolder(IFolder sourceFolder, ErrorsAndInfos errorsAndInfos) {
        List<IPosting> allPostings = [];
        List<string> files = [.. Directory.GetFiles(sourceFolder.FullName, "*.txt")];
        foreach (string file in files) {
            await dataPresenter.WriteLineAsync($"File: {file}");
            IList<IPosting> postings = sourceFileReader.ReadPostings(file, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return allPostings;
            }

            await dataPresenter.WriteLineAsync($"{postings.Count} posting/-s found");
            allPostings.AddRange(postings);
        }

        return allPostings;
    }

    private async Task<IFolder> GetSourceFolderAsync(bool isIntegrationTest) {
        IFolder sourceFolder;
        var errorsAndInfos = new ErrorsAndInfos();

        if (isIntegrationTest) {
            sourceFolder = Folders.IntegrationTestFolder;
        } else {
            CacheckConfiguration secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return null;
            }

            sourceFolder = await folderResolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
            if (!errorsAndInfos.AnyErrors()) {
                return sourceFolder;
            }

            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return null;
        }

        return sourceFolder;
    }
}