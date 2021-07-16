using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Commands;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Application {
    public class CacheckApplication : ApplicationBase<IGuiAndApplicationSynchronizer<ICacheckApplicationModel>, ICacheckApplicationModel> {
        public ICacheckHandlers Handlers { get; private set; }
        public ICacheckCommands Commands { get; private set; }

        public CacheckApplication(IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper,
            IGuiAndApplicationSynchronizer<ICacheckApplicationModel> guiAndApplicationSynchronizer, ICacheckApplicationModel model
            ) : base(buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndApplicationSynchronizer, model) {
        }

        protected override async Task EnableOrDisableButtonsAsync() {
            await Task.CompletedTask;
        }

        protected override void CreateCommandsAndHandlers() {
            Handlers = new CacheckHandlers();
            Commands = new CacheckCommands();
        }
    }
}
