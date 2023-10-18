﻿using System.Collections.Generic;
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

public class CacheckApplication : ApplicationBase<IGuiAndApplicationSynchronizer<CacheckApplicationModel>, CacheckApplicationModel>, IDataPresenter {
    public ICacheckHandlers Handlers { get; private set; }
    public bool Enabled => true;
    public ICacheckCommands Commands { get; private set; }
    public ITashHandler<CacheckApplicationModel> TashHandler { get; private set; }

    private readonly ITashAccessor _TashAccessor;
    private readonly IMethodNamesFromStackFramesExtractor _MethodNamesFromStackFramesExtractor;
    private IDataCollector _DataCollector;
    private readonly IPostingClassificationsMatcher _PostingClassificationsMatcher;

    public CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
        IGuiAndApplicationSynchronizer<CacheckApplicationModel> guiAndApplicationSynchronizer, CacheckApplicationModel model,
        ITashAccessor tashAccessor, ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor,
        IPostingClassificationsMatcher postingClassificationsMatcher)
            : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        _TashAccessor = tashAccessor;
        _MethodNamesFromStackFramesExtractor = methodNamesFromStackFramesExtractor;
        _PostingClassificationsMatcher = postingClassificationsMatcher;
    }

    protected override async Task EnableOrDisableButtonsAsync() {
        await Task.CompletedTask;
    }

    protected override void CreateCommandsAndHandlers() {
        Handlers = new CacheckHandlers {
            OverallSumsHandler = new OverallSumsHandler(Model, this),
            ClassificationSumsHandler = new ClassificationSumsHandler(Model, this),
            ClassificationAveragesHandler = new ClassificationAveragesHandler(Model, this),
            MonthlyDeltasHandler = new MonthlyDeltasHandler(Model, this),
            ClassifiedPostingsHandler = new ClassifiedPostingsHandler(Model, this),
            LogTextHandler = new CacheckTextHandler(Model, this, m => m.Log),
            SingleClassificationHandler = new SingleClassificationHandler(Model, this, () => _DataCollector, _PostingClassificationsMatcher)
        };
        Commands = new CacheckCommands();

        var selectors = new Dictionary<string, ISelector>();

        var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(_TashAccessor, SimpleLogger, _MethodNamesFromStackFramesExtractor);
        var selectorHandler = new TashSelectorHandler(Handlers, SimpleLogger, communicator, selectors, _MethodNamesFromStackFramesExtractor);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors, _MethodNamesFromStackFramesExtractor);
        TashHandler = new TashHandler(_TashAccessor, SimpleLogger, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator, _MethodNamesFromStackFramesExtractor);
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

    public override async Task OnLoadedAsync() {
        await base.OnLoadedAsync();
        await Handlers.SingleClassificationHandler.UpdateSelectableValuesAsync();
    }

    public void SetDataCollector(IDataCollector dataCollector) {
        _DataCollector = dataCollector;
    }

    public string SingleClassification() {
        return Model.SingleClassification.SelectedItem?.Name ?? "";
    }
}