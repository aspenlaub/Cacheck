using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Tash;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class TashSelectorHandler : TashSelectorHandlerBase<ICacheckApplicationModel> {
        // ReSharper disable once NotAccessedField.Local
        private readonly ICacheckHandlers vCacheckHandlers;

        public TashSelectorHandler(ICacheckHandlers cudotosiHandlers, ISimpleLogger simpleLogger, ITashCommunicator<ICacheckApplicationModel> tashCommunicator, Dictionary<string, ISelector> selectors)
                : base(simpleLogger, tashCommunicator, selectors) {
            vCacheckHandlers = cudotosiHandlers;
        }

        public override async Task ProcessSelectComboOrResetTaskAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            var controlName = status.TaskBeingProcessed.ControlName;
            SimpleLogger.LogInformation($"{controlName} is a valid selector");
            var selector = Selectors[controlName];

            await SelectedIndexChangedAsync(status, controlName, -1, false);
            if (status.TaskBeingProcessed.Status == ControllableProcessTaskStatus.BadRequest) { return; }

            var itemToSelect = status.TaskBeingProcessed.Text;
            await SelectItemAsync(status, selector, itemToSelect, controlName);
        }

        protected override async Task SelectedIndexChangedAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status, string controlName, int selectedIndex, bool selectablesChanged) {
            if (selectedIndex < 0) { return; }

            SimpleLogger.LogInformation($"Changing selected index for {controlName} to {selectedIndex}");
            switch (controlName) {
                default:
                    var errorMessage = $"Do not know how to select for {status.TaskBeingProcessed.ControlName}";
                    SimpleLogger.LogInformation($"Communicating 'BadRequest' to remote controlling process ({errorMessage})");
                    await TashCommunicator.ChangeCommunicateAndShowProcessTaskStatusAsync(status, ControllableProcessTaskStatus.BadRequest, false, "", errorMessage);
                    break;
            }
        }
    }
}
