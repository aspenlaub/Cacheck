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

        private CacheckApplication vCacheckApp;
        private ITashTimer<ICacheckApplicationModel> vTashTimer;

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
            vCacheckApp = Container.Resolve<CacheckApplication>();
            await vCacheckApp.OnLoadedAsync();

            var guiToAppGate = Container.Resolve<IGuiToApplicationGate>();
            guiToAppGate.RegisterAsyncTextBoxCallback(Summary, t => vCacheckApp.Handlers.SummaryTextHandler.TextChangedAsync(t));
            guiToAppGate.RegisterAsyncTextBoxCallback(Average, t => vCacheckApp.Handlers.AverageTextHandler.TextChangedAsync(t));
            guiToAppGate.RegisterAsyncTextBoxCallback(Log, t => vCacheckApp.Handlers.LogTextHandler.TextChangedAsync(t));

            vTashTimer = new TashTimer<ICacheckApplicationModel>(Container.Resolve<ITashAccessor>(), vCacheckApp.TashHandler, guiToAppGate);
            if (!await vTashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
                Close();
            }

            vTashTimer.CreateAndStartTimer(vCacheckApp.CreateTashTaskHandlingStatus());

            var consoleExecution = Container.Resolve<IConsoleExecution>();
            await consoleExecution.ExecuteAsync(Container, CacheckApp.IsIntegrationTest);
        }

        public void Dispose() {
            vTashTimer?.StopTimerAndConfirmDead(false);
        }

        private void OnClosing(object sender, CancelEventArgs e) {
            vTashTimer?.StopTimerAndConfirmDead(false);
        }
    }
}
