using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;
using IContainer = Autofac.IContainer;
using WindowsApplication = System.Windows.Application;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck;

/// <summary>
/// Interaction logic for CacheckWindow.xaml
/// </summary>
// ReSharper disable once UnusedMember.Global
public partial class CacheckWindow : IAsyncDisposable {
    private static IContainer Container { get; set; }

    private CacheckApplication _CacheckApp;
    private ITashTimer<CacheckApplicationModel> _TashTimer;

    public CacheckWindow() {
        InitializeComponent();

        Title = Properties.Resources.CacheckWindowTitle;
        Name = Properties.Resources.CacheckWindowName;
        AutomationProperties.SetAutomationId(this, Name);
        AutomationProperties.SetName(this, Name);
    }

    private async void OnLoaded(object sender, RoutedEventArgs e) {
        await BuildContainerIfNecessaryAsync();
        _CacheckApp = Container.Resolve<CacheckApplication>();
        await _CacheckApp.OnLoadedAsync();

        var guiToAppGate = Container.Resolve<IGuiToApplicationGate>();
        guiToAppGate.RegisterAsyncDataGridCallback(OverallSums, items => _CacheckApp.Handlers.OverallSumsHandler.CollectionChangedAsync(items));
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationSums, items => _CacheckApp.Handlers.ClassificationSumsHandler.CollectionChangedAsync(items));
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationAverages, items => _CacheckApp.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(items));
        guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDeltas, items => _CacheckApp.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(items));
        guiToAppGate.RegisterAsyncDataGridCallback(ClassifiedPostings, items => _CacheckApp.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(items));
        guiToAppGate.RegisterAsyncTextBoxCallback(Log, t => _CacheckApp.Handlers.LogTextHandler.TextChangedAsync(t));

        _TashTimer = new TashTimer<CacheckApplicationModel>(Container.Resolve<ITashAccessor>(), _CacheckApp.TashHandler, guiToAppGate);
        if (!await _TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
            Close();
        }

        _TashTimer.CreateAndStartTimer(_CacheckApp.CreateTashTaskHandlingStatus());

        var dataCollector = Container.Resolve<IDataCollector>();
        await dataCollector.CollectAndShowAsync(Container, CacheckApp.IsIntegrationTest);

        await ExceptionHandler.RunAsync(WindowsApplication.Current, TimeSpan.FromSeconds(2));
    }

    public async ValueTask DisposeAsync() {
        if (_TashTimer == null) { return; }

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);
    }

    private async void OnClosing(object sender, CancelEventArgs e) {
        e.Cancel = true;

        if (_TashTimer == null) { return; }

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);
        WindowsApplication.Current.Shutdown();
    }

    private async Task BuildContainerIfNecessaryAsync() {
        if (Container != null) { return; }

        var builder = await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(this);
        Container = builder.Build();
    }
}