using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    public class CacheckWindowUnderTest : CacheckWindowUnderTestActions, IDisposable {
        private readonly IStarterAndStopper CacheckStarterAndStopper;
        private bool IsInitialized;

        public CacheckWindowUnderTest(ITashAccessor tashAccessor, IStarterAndStopper roustStarterAndStopper) : base(tashAccessor) {
            CacheckStarterAndStopper = roustStarterAndStopper;
            IsInitialized = false;
        }

        public override async Task InitializeAsync() {
            Assert.IsFalse(IsInitialized, "Window already has been initialized");
            await base.InitializeAsync();
            CacheckStarterAndStopper.Start();
            IsInitialized = true;
        }

        public void Dispose() {
            CacheckStarterAndStopper.Stop();
        }
    }
}
