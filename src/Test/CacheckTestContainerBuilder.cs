using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test {
    public static class CacheckTestContainerBuilder {
        public static async Task<ContainerBuilder> UseCacheckVishizhukelNetAndPeghWithFakesAsync(this ContainerBuilder builder, CacheckWindow cacheckWindow) {
            await builder.UseCacheckVishizhukelNetAndPeghAsync(cacheckWindow);

            builder.RegisterType<FakeClassificationAveragePresenter>().As<IClassificationAveragePresenter>().SingleInstance();
            builder.RegisterType<FakeClassificationSumPresenter>().As<IClassificationSumPresenter>().SingleInstance();
            builder.RegisterType<FakeDataPresenter>().As<IDataPresenter>().SingleInstance();
            builder.RegisterType<FakeMonthlyDeltaPresenter>().As<IMonthlyDeltaPresenter>().SingleInstance();
            builder.RegisterType<FakeOverallSumPresenter>().As<IOverallSumPresenter>().SingleInstance();
            return builder;
        }
    }
}
