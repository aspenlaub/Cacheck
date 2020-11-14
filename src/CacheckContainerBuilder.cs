using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.PeghCore;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    public static class CacheckContainerBuilder {

        public static ContainerBuilder UseCacheckAndPegh(this ContainerBuilder builder, ICsArgumentPrompter csArgumentPrompter) {
            builder.UsePegh(csArgumentPrompter);

            return builder;
        }

        // ReSharper disable once UnusedMember.Global
        public static IServiceCollection UseCacheckAndPegh(this IServiceCollection services, ICsArgumentPrompter csArgumentPrompter) {
            services.UsePegh(csArgumentPrompter);

            return services;
        }
    }
}