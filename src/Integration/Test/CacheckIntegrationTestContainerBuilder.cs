using Aspenlaub.Net.GitHub.CSharp.Dvin.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Components;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test;

public static class CacheckIntegrationTestContainerBuilder {
    public static ContainerBuilder RegisterForCacheckIntegrationTest(this ContainerBuilder builder) {
        builder.UseDvinAndPegh("Cacheck");
        builder.RegisterType<CacheckStarterAndStopper>().As<IStarterAndStopper>();
        builder.RegisterType<CacheckWindowUnderTest>();
        builder.RegisterType<TashAccessor>().As<ITashAccessor>();
        return builder;
    }
}