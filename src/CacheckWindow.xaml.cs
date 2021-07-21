using System.Windows;
using System.Windows.Automation;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    /// <summary>
    /// Interaction logic for CacheckWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class CacheckWindow {
        private static IContainer Container { get; set; }
        private CacheckApplication vCacheckApp;

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
            guiToAppGate.RegisterAsyncTextBoxCallback(ConsoleOutput, t => vCacheckApp.Handlers.ConsoleOutputTextHandler.TextChangedAsync(t));

            var consoleExecution = Container.Resolve<IConsoleExecution>();
            await consoleExecution.ExecuteAsync(Container, CacheckApp.IsIntegrationTest);
        }
    }
}
