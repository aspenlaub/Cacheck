using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class ConsoleExecution : IConsoleExecution {
        private readonly IConsole vConsole;

        public ConsoleExecution(IConsole console) {
            vConsole = console;
        }

        public async Task ExecuteAsync(bool isIntegrationTest) {
            var container = new ContainerBuilder().UseCacheckAndPegh(new DummyCsArgumentPrompter()).Build();
            IFolder sourceFolder;
            var errorsAndInfos = new ErrorsAndInfos();
            var secretRepository = container.Resolve<ISecretRepository>();

            if (isIntegrationTest) {
                sourceFolder = Folders.IntegrationTestFolder;
            } else {
                var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                    return;
                }

                var resolver = container.Resolve<IFolderResolver>();
                sourceFolder = resolver.Resolve(secret.SourceFolder, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                    return;
                }
            }

            var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
            var reader = container.Resolve<ISourceFileReader>();
            var allPostings = new List<IPosting>();
            foreach (var file in files) {
                vConsole.WriteLine($"File: {file}");
                var postings = reader.ReadPostings(file, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                    return;
                }

                vConsole.WriteLine($"{postings.Count} posting/-s found");
                allPostings.AddRange(postings);
            }

            if (allPostings.Any()) {
                var maxDate = allPostings.Max(p => p.Date);
                var minDate = maxDate.AddYears(-1).AddDays(1);
                vConsole.WriteLine($"{allPostings.Count(p => p.Date < minDate)} posting/-s removed");
                allPostings.RemoveAll(p => p.Date < minDate);
            }

            var aggregator = container.Resolve<IPostingAggregator>();
            var pureDebitCreditAggregation = aggregator.AggregatePostings(allPostings, new List<IPostingClassification> {
                new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
                new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
            }, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                return;
            }

            vConsole.WriteLine();

            foreach (var s in
                        from result in pureDebitCreditAggregation
                        let s = result.Value.ToString("0.##")
                        select $"Sum {result.Key}: {s}") {
                vConsole.WriteLine(s);
            }

            var secret2 = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                return;
            }
            errorsAndInfos = new ErrorsAndInfos();
            var detailedAggregation = aggregator.AggregatePostings(allPostings, secret2.Cast<IPostingClassification>().ToList(), errorsAndInfos).OrderBy(a => a.Key).ToList();
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => vConsole.WriteLine(e));
                return;
            }

            vConsole.WriteLine();

            var keyLength = 0;
            if (detailedAggregation.Any()) {
                keyLength = detailedAggregation.Select(a => a.Key.Length).Max();
                foreach (var s in
                    from result in detailedAggregation
                    let s = result.Value.ToString("0.##")
                    select $"Sum {result.Key.PadRight(keyLength)}: {s}") {
                    vConsole.WriteLine(s);
                }
            }

            vConsole.WriteLine();

            foreach (var info in errorsAndInfos.Infos) {
                vConsole.WriteLine(info);
            }

            vConsole.WriteLine();

            foreach (var s in
                from result in detailedAggregation
                let s = (result.Value / 12).ToString("0.##")
                select $"Sum {result.Key.PadRight(keyLength)}: {s}") {
                vConsole.WriteLine(s);
            }
        }
    }
}