﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class TashSelectorHandler(ISimpleLogger simpleLogger,
        ITashCommunicator<ICacheckApplicationModel> tashCommunicator, Dictionary<string, ISelector> selectors,
        IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : TashSelectorHandlerBase<ICacheckApplicationModel>(simpleLogger, tashCommunicator, selectors,
                methodNamesFromStackFramesExtractor) {

    public override async Task ProcessSelectComboOrResetTaskAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        IList<string> methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        string controlName = status.TaskBeingProcessed.ControlName;
        SimpleLogger.LogInformationWithCallStack($"{controlName} is a valid selector", methodNamesFromStack);
        ISelector selector = Selectors[controlName];

        await SelectedIndexChangedAsync(status, controlName, -1, false);
        if (status.TaskBeingProcessed.Status == ControllableProcessTaskStatus.BadRequest) { return; }

        string itemToSelect = status.TaskBeingProcessed.Text;
        await SelectItemAsync(status, selector, itemToSelect, controlName);
    }

    protected override async Task SelectedIndexChangedAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status, string controlName, int selectedIndex, bool selectablesChanged) {
        if (selectedIndex < 0) { return; }

        IList<string> methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
        SimpleLogger.LogInformationWithCallStack($"Changing selected index for {controlName} to {selectedIndex}", methodNamesFromStack);
        switch (controlName) {
            default:
                string errorMessage = $"Do not know how to select for {status.TaskBeingProcessed.ControlName}";
                SimpleLogger.LogInformationWithCallStack($"Communicating 'BadRequest' to remote controlling process ({errorMessage})", methodNamesFromStack);
                await TashCommunicator.ChangeCommunicateAndShowProcessTaskStatusAsync(status, ControllableProcessTaskStatus.BadRequest, false, "", errorMessage);
                break;
        }
    }
}