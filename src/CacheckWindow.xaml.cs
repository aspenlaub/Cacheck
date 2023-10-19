using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
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
        var handlers = _CacheckApp.Handlers;
        guiToAppGate.RegisterAsyncDataGridCallback(OverallSums, handlers.OverallSumsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationSums, handlers.ClassificationSumsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationAverages, handlers.ClassificationAveragesHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDeltas, handlers.MonthlyDeltasHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassifiedPostings, handlers.ClassifiedPostingsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncTextBoxCallback(Log, handlers.LogTextHandler.TextChangedAsync);
        guiToAppGate.RegisterAsyncSelectorCallback(SingleClassification, handlers.SingleClassificationHandler.SelectedIndexChangedAsync);

        _TashTimer = new TashTimer<CacheckApplicationModel>(Container.Resolve<ITashAccessor>(), _CacheckApp.TashHandler, guiToAppGate);
        if (!await _TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
            Close();
        }

        _TashTimer.CreateAndStartTimer(_CacheckApp.CreateTashTaskHandlingStatus());

        var dataCollector = Container.Resolve<IDataCollector>();
        await dataCollector.CollectAndShowAsync(); // CacheckApp.IsIntegrationTest

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

    private async void OnChangeClassificationClickAsync(object sender, RoutedEventArgs e) {
        var postings = ClassifiedPostings.SelectedCells.Select(c => c.Item).OfType<ClassifiedPosting>().Distinct().ToList();
        if (postings.Count != 1) {
            MessageBox.Show($"{postings.Count} posting/s selected", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        if (postings[0].Amount == 0) {
            MessageBox.Show("The amount cannot be zero", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var posting = new Posting { Date = postings[0].Date, Amount = postings[0].Amount, Remark = postings[0].Remark };
        var hasher = Container.Resolve<IPostingHasher>();
        var hash = hasher.Hash(posting);
        var changeClassificationWindow = new ChangeClassificationWindow {
            Posting = posting, PostingHash = hash
        };

        var errorsAndInfos = new ErrorsAndInfos();
        var postingClassificationsSecret = await Container.Resolve<ISecretRepository>().GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show("Could find available classifications", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var credit = posting.Amount >= 0;
        var postingClassifications = postingClassificationsSecret
                .OfType<IPostingClassification>()
                .Where(c => c.Credit == credit)
                .Select(c => c.Classification)
                .Distinct()
                .Order()
                .ToList();
        changeClassificationWindow.SetClassificationChoices(postingClassifications);
        if (changeClassificationWindow.ShowDialog() != true) {
            return;
        }

        var individualPostingClassification = new IndividualPostingClassification {
            Classification = changeClassificationWindow.SelectedClassification,
            Credit = credit,
            PostingHash = changeClassificationWindow.Hash.Text
        };
        var source = Container.Resolve<IIndividualPostingClassificationsSource>();
        await source.AddOrUpdateAsync(individualPostingClassification, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show($"Could not set classification to {individualPostingClassification.Classification} for posting hash {individualPostingClassification.PostingHash}",
                Title, MessageBoxButton.OK, MessageBoxImage.Error);
        } else {
            MessageBox.Show($"Classification set to {individualPostingClassification.Classification}, please reload Cacheck",
                Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}