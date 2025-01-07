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
            LogTextHandler = new CacheckTextHandler(Model, this, m => m.Log),
            SingleClassificationHandler = new SingleClassificationHandler(Model, this, () => _DataCollector, postingClassificationsMatcher),
            LiquidityPlanSumTextHandler = new CacheckTextHandler(Model, this, m => m.LiquidityPlanSum),
            ReservationsSumTextHandler = new CacheckTextHandler(Model, this, m => m.ReservationsSum),
            MinimumAmountHandler = new MinimumAmountHandler(Model, this, () => _DataCollector, m => m.MinimumAmount)
        };
        Commands = new CacheckCommands();

        var selectors = new Dictionary<string, ISelector>();

        var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(tashAccessor, SimpleLogger, methodNamesFromStackFramesExtractor);
        var selectorHandler = new TashSelectorHandler(SimpleLogger, communicator, selectors, methodNamesFromStackFramesExtractor);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors, methodNamesFromStackFramesExtractor);
        TashHandler = new TashHandler(tashAccessor, SimpleLogger, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator, methodNamesFromStackFramesExtractor);
    }

    public ITashTaskHandlingStatus<CacheckApplicationModel> CreateTashTaskHandlingStatus() {
        return new TashTaskHandlingStatus<CacheckApplicationModel>(Model, System.Environment.ProcessId);
    }

    public string GetLogText() {
        return Model.Log.Text;
    }

    public async Task OnClassificationsFoundAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
            IList<IInverseClassificationPair> inverseClassifications) {
        await Handlers.SingleClassificationHandler.UpdateSelectableValuesAsync(classifications, postings, inverseClassifications);
    }

    public async Task OnSumsChanged(double liquidityPlanSum, double reservationsSum) {
        await Handlers.LiquidityPlanSumTextHandler.TextChangedAsync(liquidityPlanSum.ToString("F2"));
        await Handlers.ReservationsSumTextHandler.TextChangedAsync(reservationsSum.ToString("F2"));
    }

    public override async Task OnLoadedAsync() {
        await base.OnLoadedAsync();
        await Handlers.SingleClassificationHandler.UpdateSelectableValuesAsync();
        await Handlers.MinimumAmountHandler.TextChangedAsync("100");
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
}