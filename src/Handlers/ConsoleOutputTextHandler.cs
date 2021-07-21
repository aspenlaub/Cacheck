using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class ConsoleOutputTextHandler : ISimpleTextHandler {
        private readonly ICacheckApplicationModel vModel;
        private readonly IGuiAndAppHandler vGuiAndAppHandler;
        private readonly Func<ICacheckApplicationModel, ITextBox> vTextBoxGetter;

        public ConsoleOutputTextHandler(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler, Func<ICacheckApplicationModel, ITextBox> textBoxGetter) {
            vModel = model;
            vGuiAndAppHandler = guiAndAppHandler;
            vTextBoxGetter = textBoxGetter;
        }

        public async Task TextChangedAsync(string text) {
            var textBox = vTextBoxGetter(vModel);
            if (textBox.Text == text) { return; }

            textBox.Text = text;
            textBox.Type = StatusType.None;
            await vGuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        }
    }
}
