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

public class CacheckApplication : ApplicationBase<IGuiAndApplicationSynchronizer<CacheckApplicationModel>, CacheckApplicationModel>, IDataPresenter {
    public ICacheckHandlers Handlers { get; private set; }
    public ICacheckCommands Commands { get; private set; }
    public ITashHandler<CacheckApplicationModel> TashHandler { get; private set; }

    private readonly ITashAccessor TashAccessor;
    private readonly ILogConfigurationFactory LogConfigurationFactory;

    public CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
        IGuiAndApplicationSynchronizer<CacheckApplicationModel> guiAndApplicationSynchronizer, CacheckApplicationModel model,
        ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogConfigurationFactory logConfigurationFactory)
        : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model, simpleLogger) {
        TashAccessor = tashAccessor;
        LogConfigurationFactory = logConfigurationFactory;
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
            LogTextHandler = new CacheckTextHandler(Model, this, m => m.Log)
        };
        Commands = new CacheckCommands();

        var selectors = new Dictionary<string, ISelector>();

        var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(TashAccessor, SimpleLogger, LogConfigurationFactory);
        var selectorHandler = new TashSelectorHandler(Handlers, SimpleLogger, communicator, selectors);
        var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors);
        TashHandler = new TashHandler(TashAccessor, SimpleLogger, LogConfigurationFactory, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator);
    }

    public ITashTaskHandlingStatus<CacheckApplicationModel> CreateTashTaskHandlingStatus() {
        return new TashTaskHandlingStatus<CacheckApplicationModel>(Model, System.Environment.ProcessId);
    }

    public string GetLogText() {
        return Model.Log.Text;
    }
}