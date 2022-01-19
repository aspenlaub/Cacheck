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
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingCollector : IPostingCollector {
        private readonly IDataPresenter DataPresenter;

        public PostingCollector(IDataPresenter dataPresenter) {
            DataPresenter = dataPresenter;
        }

        public async Task<IList<IPosting>> CollectPostingsAsync(IContainer container, bool isIntegrationTest) {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            var allPostings = new List<IPosting>();

            var sourceFolder = await GetSourceFolderAsync(container, isIntegrationTest);
            if (sourceFolder == null) { return allPostings; }

            var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
            var reader = container.Resolve<ISourceFileReader>();
            foreach (var file in files) {
                await DataPresenter.WriteLineAsync($"File: {file}");
                var errorsAndInfos = new ErrorsAndInfos();
                var postings = reader.ReadPostings(file, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                    return allPostings;
                }

                await DataPresenter.WriteLineAsync($"{postings.Count} posting/-s found");
                allPostings.AddRange(postings);
            }

            if (!allPostings.Any()) { return allPostings; }

            var maxDate = allPostings.Max(p => p.Date);
            var minDate = new DateTime(maxDate.Year - 1, 1, 1);
            await DataPresenter.WriteLineAsync($"{allPostings.Count(p => p.Date < minDate)} posting/-s removed");
            allPostings.RemoveAll(p => p.Date < minDate);

            return allPostings;
        }

        private async Task<IFolder> GetSourceFolderAsync(IComponentContext container, bool isIntegrationTest) {
            IFolder sourceFolder;
            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();

            if (isIntegrationTest) {
                sourceFolder = Folders.IntegrationTestFolder;
            } else {
                var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                    return null;
                }

                var resolver = container.Resolve<IFolderResolver>();
                sourceFolder = await resolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
                if (!errorsAndInfos.AnyErrors()) {
                    return sourceFolder;
                }

                await DataPresenter.WriteErrorsAsync(errorsAndInfos);
                return null;
            }

            return sourceFolder;
        }
    }
}
