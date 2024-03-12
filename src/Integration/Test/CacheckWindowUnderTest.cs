using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test;

public class CacheckWindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper roustStarterAndStopper)
                : CacheckWindowUnderTestActions(tashAccessor), IDisposable {

    private bool _IsInitialized;

    public override async Task InitializeAsync() {
        Assert.IsFalse(_IsInitialized, "Window already has been initialized");
        await base.InitializeAsync();
        roustStarterAndStopper.Start();
        _IsInitialized = true;
    }

    public void Dispose() {
        roustStarterAndStopper.Stop();
    }
}