﻿using System;
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
                ITransactionIntoPostingsConverter transactionIntoPostingConverter) : IPostingCollector {

    public async Task<IList<IPosting>> CollectPostingsAsync(bool isIntegrationTest) {
        List<IPosting> allPostings = [];

        IFolder sourceFolder = await GetSourceFolderAsync(isIntegrationTest);
        if (sourceFolder == null) { return allPostings; }

        List<string> files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
        ErrorsAndInfos errorsAndInfos = new ErrorsAndInfos();
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

        IList<Transaction> transactions = await fundamentalTransactionsReader.LoadTransactionsIfAvailableAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return allPostings;
        }

        DateTime minDate = allPostings.Min(p => p.Date);
        DateTime maxDate = allPostings.Max(p => p.Date);
        transactions = transactions.Where(t
            => t.Date.Year >= minDate.Year && (t.Date.Year < maxDate.Year || t.Date.Year == maxDate.Year && t.Date.Month <= maxDate.Month)
        ).ToList();

        allPostings.AddRange(transactions.SelectMany(transactionIntoPostingConverter.Convert));

        return allPostings;
    }

    private async Task<IFolder> GetSourceFolderAsync(bool isIntegrationTest) {
        IFolder sourceFolder;
        ErrorsAndInfos errorsAndInfos = new ErrorsAndInfos();

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