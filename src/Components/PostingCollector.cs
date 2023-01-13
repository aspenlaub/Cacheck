using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingCollector : IPostingCollector {
    private readonly IDataPresenter _DataPresenter;
    private readonly ISecretRepository _SecretRepository;
    private readonly IFolderResolver _FolderResolver;
    private readonly ISourceFileReader _SourceFileReader;
    private readonly IFundamentalTransactionsReader _FundamentalTransactionsReader;
    private readonly ITransactionIntoPostingsConverter _TransactionIntoPostingConverter;

    public PostingCollector(IDataPresenter dataPresenter, ISecretRepository secretRepository, IFolderResolver folderResolver,
            ISourceFileReader sourceFileReader,
            IFundamentalTransactionsReader fundamentalTransactionsReader, ITransactionIntoPostingsConverter transactionIntoPostingConverter) {
        _DataPresenter = dataPresenter;
        _SecretRepository = secretRepository;
        _FolderResolver = folderResolver;
        _SourceFileReader = sourceFileReader;
        _FundamentalTransactionsReader = fundamentalTransactionsReader;
        _TransactionIntoPostingConverter = transactionIntoPostingConverter;
    }

    public async Task<IList<IPosting>> CollectPostingsAsync(bool isIntegrationTest) {
        var allPostings = new List<IPosting>();

        var sourceFolder = await GetSourceFolderAsync(_SecretRepository, _FolderResolver, isIntegrationTest);
        if (sourceFolder == null) { return allPostings; }

        var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
        var errorsAndInfos = new ErrorsAndInfos();
        foreach (var file in files) {
            await _DataPresenter.WriteLineAsync($"File: {file}");
            var postings = _SourceFileReader.ReadPostings(file, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return allPostings;
            }

            await _DataPresenter.WriteLineAsync($"{postings.Count} posting/-s found");
            allPostings.AddRange(postings);
        }

        var transactions = await _FundamentalTransactionsReader.LoadTransactionsIfAvailableAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return allPostings;
        }

        var maxDate = allPostings.Max(p => p.Date);

        allPostings.AddRange(transactions.SelectMany(_TransactionIntoPostingConverter.Convert));
        if (!allPostings.Any()) { return allPostings; }

        var minDate = new DateTime(maxDate.Year - 1, 1, 1);
        await _DataPresenter.WriteLineAsync($"{allPostings.Count(p => p.Date < minDate)} posting/-s removed");
        allPostings.RemoveAll(p => p.Date < minDate);

        return allPostings;
    }

    private async Task<IFolder> GetSourceFolderAsync(ISecretRepository secretRepository, IFolderResolver resolver, bool isIntegrationTest) {
        IFolder sourceFolder;
        var errorsAndInfos = new ErrorsAndInfos();

        if (isIntegrationTest) {
            sourceFolder = Folders.IntegrationTestFolder;
        } else {
            var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return null;
            }

            sourceFolder = await resolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
            if (!errorsAndInfos.AnyErrors()) {
                return sourceFolder;
            }

            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return null;
        }

        return sourceFolder;
    }
}