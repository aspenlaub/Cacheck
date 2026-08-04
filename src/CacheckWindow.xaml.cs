using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Autofac;
using IContainer = Autofac.IContainer;
using WindowsApplication = System.Windows.Application;

// ReSharper disable AsyncVoidEventHandlerMethod

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck;

/// <summary>
/// Interaction logic for CacheckWindow.xaml
/// </summary>
// ReSharper disable once UnusedMember.Global
public partial class CacheckWindow : IAsyncDisposable {
    private static IContainer Container { get; set; }

    private CacheckApplication _CacheckApp;
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private ITashTimer<CacheckApplicationModel> _TashTimer;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public CacheckWindow() {
        InitializeComponent();

        Title = Properties.Resources.CacheckWindowTitle;
        Name = Properties.Resources.CacheckWindowTitle;
        AutomationProperties.SetAutomationId(this, Name);
        AutomationProperties.SetName(this, Name);
    }

    // ReSharper disable once AsyncVoidMethod
    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void OnLoadedAsync(object sender, RoutedEventArgs e) {
        await OnLoadedAsync();
    }

    private async Task OnLoadedAsync() {
        await BuildContainerIfNecessaryAsync();

        _CacheckApp = Container.Resolve<CacheckApplication>();
        await _CacheckApp.OnLoadedAsync();

        IPostingCollector postingCollector = Container.Resolve<IPostingCollector>();
        IList<IPosting> allTimePostings = await postingCollector.CollectPostingsAsync(false);
        if (allTimePostings.AreAllPostingsPreClassified()) {
            #if DEBUG
            #else
                ClassificationsTab.Visibility = Visibility.Hidden;
            #endif
            MonthsTab.Visibility = Visibility.Hidden;
        }

        IGuiToApplicationGate guiToAppGate = Container.Resolve<IGuiToApplicationGate>();
        ICacheckHandlers handlers = _CacheckApp.Handlers;
        guiToAppGate.RegisterAsyncDataGridCallback(OverallSums, handlers.OverallSumsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationSums, handlers.ClassificationSumsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationAverages, handlers.ClassificationAveragesHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassificationTwoYearAverages, handlers.ClassificationAveragesHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDeltas, handlers.MonthlyDeltasHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(MonthlyDetails, handlers.MonthlyDetailsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(ClassifiedPostings, handlers.ClassifiedPostingsHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncDataGridCallback(SingleMonthDeltas, handlers.SingleMonthDeltasHandler.CollectionChangedAsync);
        guiToAppGate.RegisterAsyncTextBoxCallback(Log, handlers.LogTextHandler.TextChangedAsync);
        guiToAppGate.RegisterAsyncSelectorCallback(SingleClassification, handlers.SingleClassificationHandler.SelectedIndexChangedAsync);
        guiToAppGate.RegisterAsyncSelectorCallback(SingleMonth, handlers.SingleMonthHandler.SelectedIndexChangedAsync);
        guiToAppGate.RegisterAsyncTextBoxCallback(MinimumAmount, handlers.MinimumAmountHandler.TextChangedAsync);
        guiToAppGate.RegisterAsyncTextBoxCallback(FromDay, handlers.FromDayHandler.TextChangedAsync);
        guiToAppGate.RegisterAsyncTextBoxCallback(ToDay, handlers.ToDayHandler.TextChangedAsync);

        _TashTimer = new TashTimer<CacheckApplicationModel>(Container.Resolve<ITashAccessor>(), _CacheckApp.TashHandler, guiToAppGate);
        if (!await _TashTimer.ConnectAndMakeTashRegistrationReturnSuccessAsync(Properties.Resources.CacheckWindowTitle)) {
            Close();
        }

        _TashTimer.CreateAndStartTimer(_CacheckApp.CreateTashTaskHandlingStatus());

        IDataCollector dataCollector = Container.Resolve<IDataCollector>();
        await dataCollector.CollectAndShowAsync();

        await ExceptionHandler.RunAsync(WindowsApplication.Current, TimeSpan.FromSeconds(2));
    }

    public async ValueTask DisposeAsync() {
        if (_TashTimer == null) { return; }

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);

        GC.SuppressFinalize(this);
    }

    // ReSharper disable once AsyncVoidMethod
    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void OnClosingAsync(object sender, CancelEventArgs e) {
        await OnClosingAsync(e);
    }

    private async Task OnClosingAsync(CancelEventArgs e) {
        e.Cancel = true;

        if (_TashTimer == null) { return; }

        await _TashTimer.StopTimerAndConfirmDeadAsync(false);

        await SaveClassifiedPostingsAsync();

        WindowsApplication.Current.Shutdown();
    }

    private static async Task SaveClassifiedPostingsAsync() {
        IDataCollector dataCollector = Container.Resolve<IDataCollector>();
        IPostingCollector postingCollector = Container.Resolve<IPostingCollector>();
        IClassifiedPostingsCalculator calculator = Container.Resolve<IClassifiedPostingsCalculator>();
        IList<IPosting> allTimePostings = await postingCollector.CollectPostingsAsync(false);
        if (allTimePostings.AreAllPostingsPreClassified()) { return; }

        var errorsAndInfos = new ErrorsAndInfos();
        List<IPostingClassification> postingClassifications = await dataCollector.CollectPostingClassificationsAsync(errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        IList<IClassifiedPosting> classifiedPostings = await calculator.CalculateAndShowClassifiedPostingsAsync(allTimePostings,
            postingClassifications, DateTime.MinValue, 0, "", "");
        IClassifiedPostingsExporter exporter = Container.Resolve<IClassifiedPostingsExporter>();
        IFolderResolver folderResolver = Container.Resolve<IFolderResolver>();
        string exportFileFullName = await PreClassifiedPostingsSettings.ClassifiedPostingsFileFullNameAsync(folderResolver, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) { return; }

        exporter.ExportClassifiedPostings(exportFileFullName, classifiedPostings);
    }

    private async Task BuildContainerIfNecessaryAsync() {
        if (Container != null) { return; }

        ContainerBuilder builder = await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(this);
        Container = builder.Build();
    }

    private async void OnChangeClassificationClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeClassificationClickAsync(false, -1);
    }

    private async void OnChangeToFavoriteClassificationClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeClassificationClickAsync(true, 0);
    }

    private async void OnChangeToSecondFavoriteClassificationClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeClassificationClickAsync(true, 1);
    }

    private async void OnChangeToThirdFavoriteClassificationClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeClassificationClickAsync(true, 2);
    }

    private async void OnChangeToFourthFavoriteClassificationClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeClassificationClickAsync(true, 3);
    }

    private async Task OnChangeClassificationClickAsync(bool setToFavorite, int indexOfFavorite) {
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
        IPostingHasher hasher = Container.Resolve<IPostingHasher>();
        string postingHash = hasher.Hash(posting);
        ChangeClassificationWindow changeClassificationWindow = setToFavorite
            ? null
            : new ChangeClassificationWindow {
                  Posting = posting, PostingHash = postingHash
              };
        string favoriteClassification = setToFavorite
            ? (await _CacheckApp.GetIndividualPostingClassificationFavoritesAsync())[indexOfFavorite]
            : "";

        var errorsAndInfos = new ErrorsAndInfos();
        PostingClassifications postingClassificationsSecret = await Container.Resolve<ISecretRepository>().GetAsync(new PostingClassificationsSecret(), errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show("Could find available classifications", Title, MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        bool credit = posting.Amount >= 0;
        if (!setToFavorite) {
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
        }
        var individualPostingClassification = new IndividualPostingClassification {
            Classification = setToFavorite ? favoriteClassification : changeClassificationWindow.SelectedClassification,
            Credit = credit,
            PostingHash = setToFavorite ? postingHash : changeClassificationWindow.Hash.Text
        };
        IIndividualPostingClassificationsSource source = Container.Resolve<IIndividualPostingClassificationsSource>();
        await source.AddOrUpdateAsync(individualPostingClassification, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            MessageBox.Show($"Could not set classification to {individualPostingClassification.Classification} for posting hash {individualPostingClassification.PostingHash}",
                Title, MessageBoxButton.OK, MessageBoxImage.Error);
        } else {
            MessageBox.Show($"Classification set to {individualPostingClassification.Classification}, please reload Cacheck",
                Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }

    private async void OnRefreshMonthlyDetailsButtonClick(object sender, RoutedEventArgs e) {
        await OnRefreshMonthlyDetailsButtonClick();
    }

    private async Task OnRefreshMonthlyDetailsButtonClick() {
        Cursor oldCursor = Cursor;
        Cursor = Cursors.Wait;
        try {
            IDataCollector dataCollector = Container.Resolve<IDataCollector>();
            await dataCollector.CollectAndShowMonthlyDetailsAsync();
        } finally {
            Cursor = oldCursor;
        }
    }

    // ReSharper disable once AsyncVoidMethod
    // ReSharper disable once AsyncVoidEventHandlerMethod
    private async void OnChangeMonthClickAsync(object sender, RoutedEventArgs e) {
        await OnChangeMonthClickAsync();
    }

    private static async Task OnChangeMonthClickAsync() {
        await Task.Delay(TimeSpan.FromSeconds(1));
    }

    private async void ClassifiedPostings_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
        IList<string> favorites = await _CacheckApp.GetIndividualPostingClassificationFavoritesAsync();

        FavoriteClassification.Header = string.Format(Properties.Resources.ChangeClassificationTo, favorites[0]);
        SecondFavoriteClassification.Header = string.Format(Properties.Resources.ChangeClassificationTo, favorites[1]);
        ThirdFavoriteClassification.Header = string.Format(Properties.Resources.ChangeClassificationTo, favorites[2]);
        FourthFavoriteClassification.Header = string.Format(Properties.Resources.ChangeClassificationTo, favorites[3]);
    }
}