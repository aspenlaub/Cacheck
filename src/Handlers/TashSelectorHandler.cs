using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class TashSelectorHandler : TashSelectorHandlerBase<ICacheckApplicationModel> {
    // ReSharper disable once NotAccessedField.Local
    private readonly ICacheckHandlers CacheckHandlers;

    public TashSelectorHandler(ICacheckHandlers cudotosiHandlers, ISimpleLogger simpleLogger, ITashCommunicator<ICacheckApplicationModel> tashCommunicator,
            Dictionary<string, ISelector> selectors, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
        : base(simpleLogger, tashCommunicator, selectors, methodNamesFromStackFramesExtractor) {
        CacheckHandlers = cudotosiHandlers;
    }

    public override async Task ProcessSelectComboOrResetTaskAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        var controlName = status.TaskBeingProcessed.ControlName;
        SimpleLogger.LogInformationWithCallStack($"{controlName} is a valid selector", methodNamesFromStack);
        var selector = Selectors[controlName];

        await SelectedIndexChangedAsync(status, controlName, -1, false);
        if (status.TaskBeingProcessed.Status == ControllableProcessTaskStatus.BadRequest) { return; }

        var itemToSelect = status.TaskBeingProcessed.Text;
        await SelectItemAsync(status, selector, itemToSelect, controlName);
    }

    protected override async Task SelectedIndexChangedAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status, string controlName, int selectedIndex, bool selectablesChanged) {
        if (selectedIndex < 0) { return; }

        var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        SimpleLogger.LogInformationWithCallStack($"Changing selected index for {controlName} to {selectedIndex}", methodNamesFromStack);
        switch (controlName) {
            default:
                var errorMessage = $"Do not know how to select for {status.TaskBeingProcessed.ControlName}";
                SimpleLogger.LogInformationWithCallStack($"Communicating 'BadRequest' to remote controlling process ({errorMessage})", methodNamesFromStack);
                await TashCommunicator.ChangeCommunicateAndShowProcessTaskStatusAsync(status, ControllableProcessTaskStatus.BadRequest, false, "", errorMessage);
                break;
        }
    }
}