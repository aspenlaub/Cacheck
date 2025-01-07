using System;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Enums;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class MinimumAmountHandler(
        ICacheckApplicationModel model,
        IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
        Func<IDataCollector> dataCollectorGetter,
        Func<ICacheckApplicationModel, ITextBox> textBoxGetter) : ISimpleTextHandler {

    public async Task TextChangedAsync(string text) {
        ITextBox textBox = textBoxGetter(model);
        if (textBox.Text == text) { return; }

        textBox.Text = text;
        textBox.Type = StatusType.None;
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        IDataCollector dataCollector = dataCollectorGetter();
        if (dataCollector == null) { return; }

        await dataCollector.CollectAndShowAsync();
    }
}