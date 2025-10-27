using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Entities;

[TestClass]
public class CacheckConfigurationTest {
    [TestMethod]
    public async Task CanGetCacheckConfiguration() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        CacheckConfiguration secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsFalse(string.IsNullOrEmpty(secret.SourceFolder), "Source folder is empty");
        IFolderResolver resolver = container.Resolve<IFolderResolver>();
        IFolder sourceFolder = await resolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(sourceFolder.Exists(), $"Source folder \"{secret.SourceFolder}\" does not exist");
    }

    [TestMethod]
    public async Task CanGetPostingClassifications() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        PostingClassifications secret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five classifications expected");
        Assert.IsTrue(secret.Any(s => s.Credit));
        Assert.IsTrue(secret.Any(s => !s.Credit));
    }

    [TestMethod]
    public async Task CanGetInverseClassifications() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        InverseClassifications secret = await secretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five inverse classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Savings" && s.InverseClassification == "SacrificedSavings"));
    }
}