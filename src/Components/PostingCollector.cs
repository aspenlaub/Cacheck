using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Amazonian.Entities;
using Aspenlaub.Net.GitHub.CSharp.Amazonian.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingCollector(IDataPresenter dataPresenter, ISecretRepository secretRepository,
                IFolderResolver folderResolver, ISourceFileReader sourceFileReader,
                IFundamentalTransactionsReader fundamentalTransactionsReader,
                ITransactionIntoPostingsConverter transactionIntoPostingConverter,
                IClassifiedPostingsImporter importer, IAmazonianRepository amazonianRepository) : IPostingCollector {

    public async Task<IList<IPosting>> CollectPostingsAsync(bool isIntegrationTest) {
        IFolder sourceFolder = await GetSourceFolderAsync(isIntegrationTest);
        if (sourceFolder == null) { return []; }
        IFolder jsonFolder = await GetJsonFolderAsync(isIntegrationTest);
        jsonFolder?.CreateIfNecessary();

        var errorsAndInfos = new ErrorsAndInfos();
        List<IPosting> allPostings = await LoadPostingsFromSourceFolder(sourceFolder, jsonFolder, errorsAndInfos);
        allPostings = await AdjustAmazonianPostingsAsync(allPostings);
        if (allPostings.Count == 0 && !isIntegrationTest) {
            string importFileFullName = await PreClassifiedPostingsSettings.ClassifiedPostingsFileFullNameAsync(folderResolver, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) { return []; }

            allPostings = [.. await importer.ImportClassifiedPostingsAsync(importFileFullName, errorsAndInfos)];
            return errorsAndInfos.AnyErrors() ? [] : await AdjustAmazonianPostingsAsync(allPostings);
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

    private async Task<List<IPosting>> AdjustAmazonianPostingsAsync(List<IPosting> allPostings) {
        foreach (IPosting posting in allPostings.Where(p => p.Remark.Contains("amazon", StringComparison.InvariantCultureIgnoreCase))) {
            AmazonianOrder amazonianOrder = await amazonianRepository.FindOrderForPostingAsync(posting.Remark);
            if (amazonianOrder == null) {
                continue;
            }

            if (amazonianOrder.Products.Any(p => posting.Remark.Contains(p, StringComparison.Ordinal))) {
                continue;
            }

            posting.Remark = string.Join("\r\n", amazonianOrder.Products) + "\r\n" + posting.Remark;
        }
        return allPostings;
    }

    private static readonly Dictionary<string, List<IPosting>> _loadPostingsFromSourceFolderCache = new Dictionary<string, List<IPosting>>();

    private async Task<List<IPosting>> LoadPostingsFromSourceFolder(IFolder sourceFolder, IFolder jsonFolder, IErrorsAndInfos errorsAndInfos) {
        if (_loadPostingsFromSourceFolderCache.TryGetValue(sourceFolder.FullName, out List<IPosting> cachedPostings)) {
            return cachedPostings;
        }

        List<IPosting> allPostings = [];
        List<string> files = [.. Directory.GetFiles(sourceFolder.FullName, "*.txt")];
        foreach (string file in files) {
            await dataPresenter.WriteLineAsync($"File: {file}");

            string jsonFile = jsonFolder == null ? "" : file.Replace(sourceFolder.FullName, jsonFolder.FullName).Replace(".txt", ".json");
            if (File.Exists(jsonFile)) {
                List<Posting> postingsFromJson = JsonSerializer.Deserialize<List<Posting>>(await File.ReadAllTextAsync(jsonFile));
                await dataPresenter.WriteLineAsync($"{postingsFromJson.Count} posting/-s found in JSON");
                allPostings.AddRange(postingsFromJson);
                continue;
            }

            IList<IPosting> postings = sourceFileReader.ReadPostings(file, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await dataPresenter.WriteErrorsAsync(errorsAndInfos);
                return allPostings;
            }

            if (jsonFolder != null && !File.Exists(jsonFile)) {
                await File.WriteAllTextAsync(jsonFile, JsonSerializer.Serialize(postings.ToList()));
            }

            await dataPresenter.WriteLineAsync($"{postings.Count} posting/-s found");
            allPostings.AddRange(postings);
        }

        _loadPostingsFromSourceFolderCache[sourceFolder.FullName] = [.. allPostings];
        return allPostings;
    }

    private async Task<IFolder> GetSourceFolderAsync(bool isIntegrationTest) {
        IFolder sourceFolder;
        var errorsAndInfos = new ErrorsAndInfos();

        if (isIntegrationTest) {
            sourceFolder = Folders.IntegrationTestFolder;
        } else {
            CacheckConfiguration secret = await GetCacheckConfiguration(errorsAndInfos);

            sourceFolder = await folderResolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
            if (!errorsAndInfos.AnyErrors()) {
                return sourceFolder;
            }

            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return null;
        }

        return sourceFolder;
    }

    private async Task<IFolder> GetJsonFolderAsync(bool isIntegrationTest) {
        var errorsAndInfos = new ErrorsAndInfos();

        if (isIntegrationTest) {
            return null;
        }

        CacheckConfiguration secret = await GetCacheckConfiguration(errorsAndInfos);
        IFolder jsonFolder = await folderResolver.ResolveAsync(secret.JsonFolder, errorsAndInfos);
        if (!errorsAndInfos.AnyErrors()) {
            return jsonFolder;
        }

        await dataPresenter.WriteErrorsAsync(errorsAndInfos);
        return null;
    }

    private async Task<CacheckConfiguration> GetCacheckConfiguration(IErrorsAndInfos errorsAndInfos) {
        CacheckConfiguration secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
        if (!errorsAndInfos.AnyErrors()) {
            return secret;
        }

        await dataPresenter.WriteErrorsAsync(errorsAndInfos);
        return secret;

    }
}