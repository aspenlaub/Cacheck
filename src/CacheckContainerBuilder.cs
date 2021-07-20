using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    public static class CacheckContainerBuilder {

        public static ContainerBuilder UseCacheckAndPegh(this ContainerBuilder builder, ICsArgumentPrompter csArgumentPrompter) {
            builder.UsePegh(csArgumentPrompter);
            builder.RegisterType<WindowsConsole>().As<IConsole>();
            builder.RegisterType<ConsoleExecution>().As<IConsoleExecution>();
            builder.RegisterType<SourceFileReader>().As<ISourceFileReader>();
            builder.RegisterType<PostingAggregator>().As<IPostingAggregator>();
            return builder;
        }
    }
}