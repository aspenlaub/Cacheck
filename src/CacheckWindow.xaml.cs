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

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    /// <summary>
    /// Interaction logic for CacheckWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class CacheckWindow : IAsyncDisposable {
        private static IContainer Container { get; set; }

        private CacheckApplication CacheckApp;
        private ITashTimer<CacheckApplicationModel> TashTimer;

        public CacheckWindow() {
            InitializeComponent();

            Title = Properties.Resources.CacheckWindowTitle;
            Name = Properties.Resources.CacheckWindowName;
            AutomationProperties.SetAutomationId(this, Name);
            AutomationProperties.SetName(this, Name);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e) {
            await BuildContainerIfNecessaryAsync();
            CacheckApp = Container.Resolve<CacheckApplication>();
            await CacheckApp.OnLoadedAsync();

            var guiToAppGate = Container.Resolve<IGuiToApplicationGate>();
            guiToAppGate.RegisterAsyncDataGridCallback(OverallSums, items => CacheckApp.Handlers.OverallSumsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassificationSums, items => CacheckApp.Handlers.ClassificationSumsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassificationAverages, items => CacheckApp.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDeltas, items => CacheckApp.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassifiedPostings, items => CacheckApp.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncTextBoxCallback(Log, t => CacheckApp.Handlers.LogTextHandler.TextChangedAsync(t));

            TashTimer = new TashTimer<CacheckApplicationModel>(Container.Resolve<ITashAccessor>(), CacheckApp.TashHandler, guiToAppGate);
            if (!await TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
                Close();
            }

            TashTimer.CreateAndStartTimer(CacheckApp.CreateTashTaskHandlingStatus());

            var dataCollector = Container.Resolve<IDataCollector>();
            await dataCollector.CollectAndShowAsync(Container, Cacheck.CacheckApp.IsIntegrationTest);

            await ExceptionHandler.RunAsync(WindowsApplication.Current, TimeSpan.FromSeconds(2));
        }

        public async ValueTask DisposeAsync() {
            if (TashTimer == null) { return; }

            await TashTimer.StopTimerAndConfirmDeadAsync(false);
        }

        private async void OnClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;

            if (TashTimer == null) { return; }

            await TashTimer.StopTimerAndConfirmDeadAsync(false);
            WindowsApplication.Current.Shutdown();
        }

        private async Task BuildContainerIfNecessaryAsync() {
            if (Container != null) { return; }

            var builder = await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(this);
            Container = builder.Build();
        }
    }
}
