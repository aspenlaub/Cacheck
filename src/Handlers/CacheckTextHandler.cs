using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class CacheckTextHandler : ISimpleTextHandler {
    private readonly ICacheckApplicationModel _Model;
    private readonly IGuiAndAppHandler<CacheckApplicationModel> _GuiAndAppHandler;
    private readonly Func<ICacheckApplicationModel, ITextBox> _TextBoxGetter;

    public CacheckTextHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler, Func<ICacheckApplicationModel, ITextBox> textBoxGetter) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
        _TextBoxGetter = textBoxGetter;
    }

    public async Task TextChangedAsync(string text) {
        var textBox = _TextBoxGetter(_Model);
        if (textBox.Text == text) { return; }

        textBox.Text = text;
        textBox.Type = StatusType.None;
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }
}