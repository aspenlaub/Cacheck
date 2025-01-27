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

    [TestMethod]
    public async Task CanGetIrregularDebitClassifications() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        IrregularDebitClassifications secret = await secretRepository.GetAsync(new IrregularDebitClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five irregular debit classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Holiday" && s.Percentage == 100));
    }

    [TestMethod]
    public async Task CanGetLiquidityPlanClassifications() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISecretRepository secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        LiquidityPlanClassifications secret = await secretRepository.GetAsync(new LiquidityPlanClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five irregular debit classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Holiday" && s.LiquidityClassification == "Fix" && s.Percentage == 100));
    }
}