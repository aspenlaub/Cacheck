using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core {
    public static class CacheckContainerBuilder {

        public static ContainerBuilder UseCacheckAndPegh(this ContainerBuilder builder) {
            builder.UsePegh(new DummyCsArgumentPrompter());
            builder.RegisterType<SourceFileReader>().As<ISourceFileReader>();
            builder.RegisterType<PostingAggregator>().As<IPostingAggregator>();
            return builder;
        }
    }
}