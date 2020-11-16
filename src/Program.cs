using System;
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

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    internal class Program {
        private static async Task Main() {
            var container = new ContainerBuilder().UseCacheckAndPegh(new DummyCsArgumentPrompter()).Build();
            var secretRepository = container.Resolve<ISecretRepository>();
            var errorsAndInfos = new ErrorsAndInfos();
            var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                return;
            }

            var resolver = container.Resolve<IFolderResolver>();
            var sourceFolder = resolver.Resolve(secret.SourceFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                return;
            }

            var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
            var reader = container.Resolve<ISourceFileReader>();
            var allPostings = new List<IPosting>();
            foreach (var file in files) {
                Console.WriteLine($"File: {file}");
                var postings = reader.ReadPostings(file, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                    return;
                }

                Console.WriteLine($"{postings.Count} posting/-s found");
                allPostings.AddRange(postings);
            }

            var aggregator = container.Resolve<IPostingAggregator>();
            var pureDebitCreditAggregation = aggregator.AggregatePostings(allPostings, new List<IPostingClassification> {
                new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
                new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
            }, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                return;
            }

            Console.WriteLine();

            foreach (var s in
                        from result in pureDebitCreditAggregation
                        let s = result.Value.ToString("0.##")
                        select $"Sum {result.Key}: {s}") {
                Console.WriteLine(s);
            }

            var secret2 = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                return;
            }
            var detailedAggregation = aggregator.AggregatePostings(allPostings, secret2.Cast<IPostingClassification>().ToList(), errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                return;
            }

            Console.WriteLine();

            foreach (var s in
                from result in detailedAggregation
                let s = result.Value.ToString("0.##")
                select $"Sum {result.Key}: {s}") {
                Console.WriteLine(s);
            }

        }
    }
}
