using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Handlers {
    public class ConsoleOutputTextHandler : ISimpleTextHandler {
        private readonly ICacheckApplicationModel vModel;
        private readonly IGuiAndAppHandler vGuiAndAppHandler;

        public ConsoleOutputTextHandler(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler) {
            vModel = model;
            vGuiAndAppHandler = guiAndAppHandler;
        }

        public async Task TextChangedAsync(string text) {
            if (vModel.ConsoleOutput.Text == text) { return; }

            vModel.ConsoleOutput.Text = text;
            vModel.ConsoleOutput.Type = StatusType.None;
            await vGuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        }
    }
}
