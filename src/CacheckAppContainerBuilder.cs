using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck;

public static class CacheckAppContainerBuilder {
    public static async Task<ContainerBuilder> UseCacheckVishizhukelNetAndPeghAsync(this ContainerBuilder builder, CacheckWindow cacheckWindow) {
        await builder.UseVishizhukelNetDvinAndPeghAsync("Cacheck", new DummyCsArgumentPrompter());

        builder.RegisterType<AverageCalculator>().As<IAverageCalculator>();
        builder.RegisterType<AggregatedPostingsNetter>().As<IAggregatedPostingsNetter>();
        builder.RegisterType<CacheckApplication>().As<CacheckApplication>().As<IGuiAndAppHandler<CacheckApplicationModel>>().As<IDataPresenter>().SingleInstance();
        builder.RegisterType<CacheckApplicationModel>().As<CacheckApplicationModel>().As<ICacheckApplicationModel>().As<IApplicationModelBase>().As<IBusy>().SingleInstance();
        builder.RegisterType<CacheckGuiAndApplicationSynchronizer>()
            .WithParameter((p, _) => p.ParameterType == typeof(CacheckWindow), (_, _) => cacheckWindow)
            .As<IGuiAndApplicationSynchronizer<CacheckApplicationModel>>();
        builder.RegisterType<CacheckGuiToApplicationGate>().As<IGuiToApplicationGate>().SingleInstance();
        builder.RegisterType<CalculationLogger>().As<ICalculationLogger>().SingleInstance();
        builder.RegisterType<ClassifiedPosting>().As<IClassifiedPosting>();
        builder.RegisterType<ClassifiedPostingsCalculator>().As<IClassifiedPostingsCalculator>();
        builder.RegisterType<ClassifiedPostingsExporter>().As<IClassifiedPostingsExporter>();
        builder.RegisterType<DataCollector>()
               .WithParameter((p, _) => p.Name == "isIntegrationTest", (_, _) => CacheckApp.IsIntegrationTest)
               .As<IDataCollector>();
        builder.RegisterType<FormattedClassificationComparer>().As<IFormattedClassificationComparer>();
        builder.RegisterType<FundamentalTransactionsReader>().As<IFundamentalTransactionsReader>();
        builder.RegisterType<IndividualPostingClassificationConverter>().As<IIndividualPostingClassificationConverter>();
        builder.RegisterType<IndividualPostingClassificationsSource>().As<IIndividualPostingClassificationsSource>();
        builder.RegisterType<IndividualPostingEliminationAnalyzer>().As<IIndividualPostingEliminationAnalyzer>();
        builder.RegisterType<MonthlyDeltaCalculator>().As<IMonthlyDeltaCalculator>();
        builder.RegisterType<MonthlyDetailsCalculator>().As<IMonthlyDetailsCalculator>();
        builder.RegisterType<PostingAggregator>().As<IPostingAggregator>();
        builder.RegisterType<PostingClassificationFormatter>().As<IPostingClassificationFormatter>();
        builder.RegisterType<PostingClassificationMatcher>().As<IPostingClassificationMatcher>();
        builder.RegisterType<PostingClassificationsMatcher>().As<IPostingClassificationsMatcher>();
        builder.RegisterType<PostingCollector>().As<IPostingCollector>();
        builder.RegisterType<PostingHasher>().As<IPostingHasher>();
        builder.RegisterType<PotentialCueExtractor>().As<IPotentialCueExtractor>();
        builder.RegisterType<SecretRepositoryFactory>().As<ISecretRepositoryFactory>();
        builder.RegisterType<SingleMonthDeltasCalculator>().As<ISingleMonthDeltasCalculator>();
        builder.RegisterType<SourceFileReader>().As<ISourceFileReader>();
        builder.RegisterType<SummaryCalculator>().As<ISummaryCalculator>();
        builder.RegisterType<TransactionIntoPostingsConverter>().As<ITransactionIntoPostingsConverter>();
        builder.RegisterType<TypeItemSum>().As<ITypeItemSum>();
        builder.RegisterType<TypeMonthDelta>().As<ITypeMonthDelta>();
        return builder;
    }
}