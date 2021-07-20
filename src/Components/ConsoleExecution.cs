using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class ConsoleExecution : IConsoleExecution {
        private readonly IConsole vConsole;

        public ConsoleExecution(IConsole console) {
            vConsole = console;
        }

        public async Task ExecuteAsync(IContainer container, bool isIntegrationTest) {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }
            IFolder sourceFolder;
            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();

            if (isIntegrationTest) {
                sourceFolder = Folders.IntegrationTestFolder;
            } else {
                var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await WriteErrorsAsync(errorsAndInfos);
                    return;
                }

                var resolver = container.Resolve<IFolderResolver>();
                sourceFolder = await resolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await WriteErrorsAsync(errorsAndInfos);
                    return;
                }
            }

            var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
            var reader = container.Resolve<ISourceFileReader>();
            var allPostings = new List<IPosting>();
            foreach (var file in files) {
                await vConsole.WriteLineAsync($"File: {file}");
                var postings = reader.ReadPostings(file, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    await WriteErrorsAsync(errorsAndInfos);
                    return;
                }

                await vConsole.WriteLineAsync($"{postings.Count} posting/-s found");
                allPostings.AddRange(postings);
            }

            if (allPostings.Any()) {
                var maxDate = allPostings.Max(p => p.Date);
                var minDate = maxDate.AddYears(-1).AddDays(1);
                await vConsole.WriteLineAsync($"{allPostings.Count(p => p.Date < minDate)} posting/-s removed");
                allPostings.RemoveAll(p => p.Date < minDate);
            }

            var aggregator = container.Resolve<IPostingAggregator>();
            var pureDebitCreditAggregation = aggregator.AggregatePostings(allPostings, new List<IPostingClassification> {
                new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
                new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
            }, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await WriteErrorsAsync(errorsAndInfos);
                return;
            }

            await vConsole.WriteLineAsync();

            foreach (var s in
                        from result in pureDebitCreditAggregation
                        let s = result.Value.ToString("0.##")
                        select $"Sum {result.Key}: {s}") {
                await vConsole.WriteLineAsync(s);
            }

            var secret2 = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                await WriteErrorsAsync(errorsAndInfos);
                return;
            }
            errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = aggregator.AggregatePostings(allPostings, secret2.Cast<IPostingClassification>().ToList(), errorsAndInfos).OrderBy(a => a.Key).ToList();
            if (errorsAndInfos.AnyErrors()) {
                await WriteErrorsAsync(errorsAndInfos);
                return;
            }

            await vConsole.WriteLineAsync();

            var keyLength = 0;
            if (detailedAggregation.Any()) {
                keyLength = detailedAggregation.Select(a => a.Key.Length).Max();
                foreach (var s in
                    from result in detailedAggregation
                    let s = result.Value.ToString("0.##")
                    select $"Sum {result.Key.PadRight(keyLength)}: {s}") {
                    await vConsole.WriteLineAsync(s);
                }
            }

            await vConsole.WriteLineAsync();

            foreach (var info in errorsAndInfos.Infos) {
                await vConsole.WriteLineAsync(info);
            }

            await vConsole.WriteLineAsync();

            foreach (var s in
                from result in detailedAggregation
                let s = (result.Value / 12).ToString("0.##")
                select $"Sum {result.Key.PadRight(keyLength)}: {s}") {
                await vConsole.WriteLineAsync(s);
            }
        }

        protected async Task WriteErrorsAsync(IErrorsAndInfos errorsAndInfos) {
            var errors = errorsAndInfos.Errors.ToList();
            foreach (var error in errors) {
                await vConsole.WriteLineAsync(error);
            }
        }
    }
}