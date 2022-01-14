using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;
using IContainer = Autofac.IContainer;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    /// <summary>
    /// Interaction logic for CacheckWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class CacheckWindow : IDisposable {
        private static IContainer Container { get; set; }

        private CacheckApplication CacheckApp;
        private ITashTimer<ICacheckApplicationModel> TashTimer;

        public CacheckWindow() {
            InitializeComponent();

            var builder = new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(this).Result;
            Container = builder.Build();

            Title = Properties.Resources.CacheckWindowTitle;
            Name = Properties.Resources.CacheckWindowName;
            AutomationProperties.SetAutomationId(this, Name);
            AutomationProperties.SetName(this, Name);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e) {
            CacheckApp = Container.Resolve<CacheckApplication>();
            await CacheckApp.OnLoadedAsync();

            var guiToAppGate = Container.Resolve<IGuiToApplicationGate>();
            guiToAppGate.RegisterAsyncDataGridCallback(OverallSums, items => CacheckApp.Handlers.OverallSumsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassificationSums, items => CacheckApp.Handlers.ClassificationSumsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassificationAverages, items => CacheckApp.Handlers.ClassificationAveragesHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDeltas, items => CacheckApp.Handlers.MonthlyDeltasHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(PostingAdjustments, items => CacheckApp.Handlers.PostingAdjustmentsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncDataGridCallback(ClassifiedPostings, items => CacheckApp.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(items));
            guiToAppGate.RegisterAsyncTextBoxCallback(Log, t => CacheckApp.Handlers.LogTextHandler.TextChangedAsync(t));

            TashTimer = new TashTimer<ICacheckApplicationModel>(Container.Resolve<ITashAccessor>(), CacheckApp.TashHandler, guiToAppGate);
            if (!await TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
                Close();
            }

            TashTimer.CreateAndStartTimer(CacheckApp.CreateTashTaskHandlingStatus());

            var dataCollector = Container.Resolve<IDataCollector>();
            await dataCollector.CollectAndShowAsync(Container, Cacheck.CacheckApp.IsIntegrationTest);
        }

        public void Dispose() {
            TashTimer?.StopTimerAndConfirmDead(false);
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            TashTimer?.StopTimerAndConfirmDead(false);
        }
    }
}
