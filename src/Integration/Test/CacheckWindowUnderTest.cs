using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test;

public class CacheckWindowUnderTest : CacheckWindowUnderTestActions, IDisposable {
    private readonly IStarterAndStopper _CacheckStarterAndStopper;
    private bool _IsInitialized;

    public CacheckWindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper roustStarterAndStopper) : base(tashAccessor) {
        _CacheckStarterAndStopper = roustStarterAndStopper;
        _IsInitialized = false;
    }

    public override async Task InitializeAsync() {
        Assert.IsFalse(_IsInitialized, "Window already has been initialized");
        await base.InitializeAsync();
        _CacheckStarterAndStopper.Start();
        _IsInitialized = true;
    }

    public void Dispose() {
        _CacheckStarterAndStopper.Stop();
    }
}