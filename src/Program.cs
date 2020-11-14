using System;
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
            }

            var resolver = container.Resolve<IFolderResolver>();
            var sourceFolder = resolver.Resolve(secret.SourceFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
            }

            var files = Directory.GetFiles(sourceFolder.FullName, "*.txt").ToList();
            var reader = container.Resolve<ISourceFileReader>();
            foreach (var file in files) {
                Console.WriteLine($"File: {file}");
                var postings = reader.ReadPostings(file, errorsAndInfos);
                if (errorsAndInfos.AnyErrors()) {
                    errorsAndInfos.Errors.ToList().ForEach(e => Console.WriteLine(e));
                }

                Console.WriteLine($"{postings.Count} posting/-s found");
            }
        }
    }
}
