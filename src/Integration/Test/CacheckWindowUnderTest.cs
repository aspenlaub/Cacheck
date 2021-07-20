using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    public class CacheckWindowUnderTest : CacheckWindowUnderTestActions, IDisposable {
        private readonly IStarterAndStopper vCacheckStarterAndStopper;
        private bool vInitialized;

        public CacheckWindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper roustStarterAndStopper) : base(tashAccessor) {
            vCacheckStarterAndStopper = roustStarterAndStopper;
            vInitialized = false;
        }

        public override async Task InitializeAsync() {
            Assert.IsFalse(vInitialized, "Window already has been initialized");
            await base.InitializeAsync();
            vCacheckStarterAndStopper.Start();
            vInitialized = true;
        }

        public void Dispose() {
            vCacheckStarterAndStopper.Stop();
        }
    }
}
