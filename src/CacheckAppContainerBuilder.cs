using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Helpers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    public static class CacheckAppContainerBuilder {
        public static async Task<ContainerBuilder> UseCacheckVishizhukelNetAndPeghAsync(this ContainerBuilder builder, CacheckWindow cacheckWindow) {
            await builder.UseVishizhukelNetDvinAndPeghAsync(new DummyCsArgumentPrompter(), new LogConfiguration());

            builder.RegisterType<AverageCalculator>().As<IAverageCalculator>();
            builder.RegisterType<CacheckApplication>().As<CacheckApplication>().As<IGuiAndAppHandler>().As<IDataPresenter>().SingleInstance();
            builder.RegisterType<CacheckApplicationModel>().As<CacheckApplicationModel>().As<ICacheckApplicationModel>().As<IApplicationModel>().As<IBusy>().SingleInstance();
            builder.RegisterType<CacheckGuiAndApplicationSynchronizer>().WithParameter((p, _) => p.ParameterType == typeof(CacheckWindow), (_, _) => cacheckWindow).As<IGuiAndApplicationSynchronizer<ICacheckApplicationModel>>();
            builder.RegisterType<CacheckGuiToApplicationGate>().As<IGuiToApplicationGate>().SingleInstance();
            builder.RegisterType<DataCollector>().As<IDataCollector>();
            builder.RegisterType<PostingAggregator>().As<IPostingAggregator>();
            builder.RegisterType<PostingCollector>().As<IPostingCollector>();
            builder.RegisterType<SourceFileReader>().As<ISourceFileReader>();
            builder.RegisterType<SummaryCalculator>().As<ISummaryCalculator>();
            return builder;
        }
    }
}