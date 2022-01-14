using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Commands;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Application {
    public class CacheckApplication : ApplicationBase<IGuiAndApplicationSynchronizer<ICacheckApplicationModel>, ICacheckApplicationModel>, IDataPresenter {
        public ICacheckHandlers Handlers { get; private set; }
        public ICacheckCommands Commands { get; private set; }
        public ITashHandler<ICacheckApplicationModel> TashHandler { get; private set; }

        private readonly ITashAccessor TashAccessor;
        private readonly ISimpleLogger SimpleLogger;
        private readonly ILogConfiguration LogConfiguration;

        public CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            IGuiAndApplicationSynchronizer<ICacheckApplicationModel> guiAndApplicationSynchronizer, ICacheckApplicationModel model,
            ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration
            ) : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model) {
            TashAccessor = tashAccessor;
            SimpleLogger = simpleLogger;
            LogConfiguration = logConfiguration;
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
                PostingAdjustmentsHandler = new PostingAdjustmentsHandler(Model, this),
                ClassifiedPostingsHandler = new ClassifiedPostingsHandler(Model, this),
                LogTextHandler = new CacheckTextHandler(Model, this, m => m.Log)
            };
            Commands = new CacheckCommands();

            var selectors = new Dictionary<string, ISelector>();

            var communicator = new TashCommunicatorBase<ICacheckApplicationModel>(TashAccessor, SimpleLogger, LogConfiguration);
            var selectorHandler = new TashSelectorHandler(Handlers, SimpleLogger, communicator, selectors);
            var verifyAndSetHandler = new TashVerifyAndSetHandler(Handlers, SimpleLogger, selectorHandler, communicator, selectors);
            TashHandler = new TashHandler(TashAccessor, SimpleLogger, LogConfiguration, ButtonNameToCommandMapper, ToggleButtonNameToHandlerMapper, this, verifyAndSetHandler, selectorHandler, communicator);
        }

        public ITashTaskHandlingStatus<ICacheckApplicationModel> CreateTashTaskHandlingStatus() {
            return new TashTaskHandlingStatus<ICacheckApplicationModel>(Model, System.Environment.ProcessId);
        }

        public string GetLogText() {
            return Model.Log.Text;
        }
    }
}
