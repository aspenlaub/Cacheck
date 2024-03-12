using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class CacheckTextHandler(ICacheckApplicationModel model,
                IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
                Func<ICacheckApplicationModel, ITextBox> textBoxGetter) : ISimpleTextHandler {

    public async Task TextChangedAsync(string text) {
        var textBox = textBoxGetter(model);
        if (textBox.Text == text) { return; }

        textBox.Text = text;
        textBox.Type = StatusType.None;
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}