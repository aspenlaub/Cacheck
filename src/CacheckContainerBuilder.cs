using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    public static class CacheckContainerBuilder {

        public static ContainerBuilder UseCacheckAndPegh(this ContainerBuilder builder) {
            builder.UsePegh(new DummyCsArgumentPrompter());
            builder.RegisterType<SourceFileReader>().As<ISourceFileReader>();
            builder.RegisterType<PostingAggregator>().As<IPostingAggregator>();
            return builder;
        }
    }
}