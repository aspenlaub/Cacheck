using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Commands;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;

public class CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper,
                IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
                IGuiAndApplicationSynchronizer<CacheckApplicationModel> guiAndApplicationSynchronizer,
                CacheckApplicationModel model, ITashAccessor tashAccessor, ISimpleLogger simpleLogger,
                IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor,
                IPostingClassificationsMatcher postingClassificationsMatcher)
                    : ApplicationBase<IGuiAndApplicationSynchronizer<CacheckApplicationModel>,
                          CacheckApplicationModel>(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper,
                            guiAndApplicationSynchronizer, model, simpleLogger), IDataPresenter {

    public ICacheckHandlers Handlers { get; private set; }
    public bool Enabled => true;
    public ICacheckCommands Commands { get; private set; }
    public ITashHandler<CacheckApplicationModel> TashHandler { get; private set; }

    private IDataCollector _DataCollector;

    protected override async Task EnableOrDisableButtonsAsync() {
        await Task.CompletedTask;
    }

    protected override void CreateCommandsAndHandlers() {
        Handlers = new CacheckHandlers {
            OverallSumsHandler = new OverallSumsHandler(Model, this),
            ClassificationSumsHandler = new ClassificationSumsHandler(Model, this),
            ClassificationAveragesHandler = new ClassificationAveragesHandler(Model, this),
            MonthlyDeltasHandler = new MonthlyDeltasHandler(Model, this),
            MonthlyDetailsHandler = new MonthlyDetailsHandler(Model, this),
            ClassifiedPostingsHandler = new ClassifiedPostingsHandler(Model, this),
            SingleMonthDeltasHandler = new SingleMonthDeltasHandler(Model, this),
            LogTextHandler = new CacheckTextHandler(Model, this, m => m.Log),
            SingleClassificationHandler = new SingleClassificationHandler(Model, this, () => _DataCollector, postingClassificationsMatcher),
            SingleMonthHandler = new SingleMonthHandler(Model, this, () => _DataCollector),
            LiquidityPlanSumTextHandler = new CacheckTextHandler(Model, this, m => m.LiquidityPlanSum),
            ReservationsSumTextHandler = new CacheckTextHandler(Model, this, m => m.ReservationsSum),
            MinimumAmountHandler = new CacheckTextHandler(Model, this, m => m.MinimumAmount),
            FromDayHandler = new CacheckTextHandler(Model, this, m => m.FromDay),
            ToDayHandler = new CacheckTextHandler(Model, this, m => m.ToDay)
        };
        Commands = new CacheckCommands();

        var selectors = new Dictionary<string, ISelector>();

        var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(tashAccessor, SimpleLogger, methodNamesFromStackFramesExtractor);
        var selectorHandler = new TashSelectorHandler(SimpleLogger, communicator, selectors, methodNamesFromStackFramesExtractor);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors, methodNamesFromStackFramesExtractor);
        TashHandler = new TashHandler(tashAccessor, SimpleLogger, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator, methodNamesFromStackFramesExtractor);
    }

    public ITashTaskHandlingStatus<CacheckApplicationModel> CreateTashTaskHandlingStatus() {
        return new TashTaskHandlingStatus<CacheckApplicationModel>(Model, Environment.ProcessId);
    }

    public string GetLogText() {
        return Model.Log.Text;
    }

    public async Task OnClassificationsFoundAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
            IList<IInverseClassificationPair> inverseClassifications, bool areWeCollecting) {
        await Handlers.SingleClassificationHandler.UpdateSelectableValuesAsync(classifications, postings, inverseClassifications, areWeCollecting);
    }

    public async Task OnSumsChanged(double liquidityPlanSum, double reservationsSum) {
        await Handlers.LiquidityPlanSumTextHandler.TextChangedAsync(liquidityPlanSum.ToString("F2"));
        await Handlers.ReservationsSumTextHandler.TextChangedAsync(reservationsSum.ToString("F2"));
    }

    public override async Task OnLoadedAsync() {
        await base.OnLoadedAsync();
        await Handlers.SingleClassificationHandler.UpdateSelectableValuesAsync(false);
        await Handlers.SingleMonthHandler.UpdateSelectableValuesAsync();
        await Handlers.MinimumAmountHandler.TextChangedAsync("100");
        int day = DateTime.Today.Day;
        await Handlers.FromDayHandler.TextChangedAsync(day.ToString());
        day = Math.Min(27, Math.Max(day + 5, 31));
        await Handlers.ToDayHandler.TextChangedAsync(day.ToString());
    }

    public void SetDataCollector(IDataCollector dataCollector) {
        _DataCollector = dataCollector;
    }

    public string SingleClassification() {
        return Model.SingleClassification.SelectedItem?.Name ?? "";
    }

    public double MinimumAmount() {
        return double.TryParse(Model.MinimumAmount.Text, out double minimumAmount) ? minimumAmount : 0;
    }

    public int FromDay() {
        return int.TryParse(Model.FromDay.Text, out int fromDay) ? fromDay : 1;
    }

    public int ToDay() {
        return int.TryParse(Model.ToDay.Text, out int toDay) ? toDay : 31;
    }

    public int SingleMonthNumber() {
        return int.TryParse(Model.SingleMonth.SelectedItem?.Guid, out int monthNumber) ? monthNumber : 0;
    }
}