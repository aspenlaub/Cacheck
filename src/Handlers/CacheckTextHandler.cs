using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class CacheckTextHandler : ISimpleTextHandler {
        private readonly ICacheckApplicationModel Model;
        private readonly IGuiAndAppHandler<CacheckApplicationModel> GuiAndAppHandler;
        private readonly Func<ICacheckApplicationModel, ITextBox> TextBoxGetter;

        public CacheckTextHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler, Func<ICacheckApplicationModel, ITextBox> textBoxGetter) {
            Model = model;
            GuiAndAppHandler = guiAndAppHandler;
            TextBoxGetter = textBoxGetter;
        }

        public async Task TextChangedAsync(string text) {
            var textBox = TextBoxGetter(Model);
            if (textBox.Text == text) { return; }

            textBox.Text = text;
            textBox.Type = StatusType.None;
            await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        }
    }
}
