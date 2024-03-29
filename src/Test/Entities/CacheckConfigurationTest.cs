﻿using System.Linq;
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
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        var secret = await secretRepository.GetAsync(new CacheckConfigurationSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsFalse(string.IsNullOrEmpty(secret.SourceFolder), "Source folder is empty");
        var resolver = container.Resolve<IFolderResolver>();
        var sourceFolder = await resolver.ResolveAsync(secret.SourceFolder, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(sourceFolder.Exists(), $"Source folder \"{secret.SourceFolder}\" does not exist");
    }

    [TestMethod]
    public async Task CanGetPostingClassifications() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        var secret = await secretRepository.GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five classifications expected");
        Assert.IsTrue(secret.Any(s => s.Credit));
        Assert.IsTrue(secret.Any(s => !s.Credit));
    }

    [TestMethod]
    public async Task CanGetInverseClassifications() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        var secret = await secretRepository.GetAsync(new InverseClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five inverse classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Savings" && s.InverseClassification == "SacrificedSavings"));
    }

    [TestMethod]
    public async Task CanGetIrregularDebitClassifications() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        var secret = await secretRepository.GetAsync(new IrregularDebitClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five irregular debit classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Holiday" && s.Percentage == 100));
    }

    [TestMethod]
    public async Task CanGetLiquidityPlanClassifications() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var secretRepository = container.Resolve<ISecretRepository>();
        var errorsAndInfos = new ErrorsAndInfos();
        var secret = await secretRepository.GetAsync(new LiquidityPlanClassificationsSecret(), errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(secret.Count >= 5, "At least five irregular debit classifications expected");
        Assert.IsTrue(secret.Any(s => s.Classification == "Holiday" && s.LiquidityClassification == "Fix" && s.Percentage == 100));
    }
}