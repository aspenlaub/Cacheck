using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

public class CalculatorTestBase {
    protected static readonly PostingTestData TestData = new();

    protected IContainer Container;
    protected FakeDataPresenter FakeDataPresenter;

    protected void InitializeContainerAndDataPresenter() {
        Container = new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null).Result.Build();
        FakeDataPresenter = Container.Resolve<IDataPresenter>() as FakeDataPresenter;
        Assert.IsNotNull(FakeDataPresenter);
    }
}