﻿using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Enums;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class TashHandler : TashHandlerBase<ICacheckApplicationModel> {
        public TashHandler(ITashAccessor tashAccessor, ISimpleLogger simpleLogger, ILogConfiguration logConfiguration,
            IButtonNameToCommandMapper buttonNameToCommandMapper, IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper, IGuiAndAppHandler guiAndAppHandler,
            ITashVerifyAndSetHandler<ICacheckApplicationModel> tashVerifyAndSetHandler, ITashSelectorHandler<ICacheckApplicationModel> tashSelectorHandler, ITashCommunicator<ICacheckApplicationModel> tashCommunicator)
            : base(tashAccessor, simpleLogger, logConfiguration, buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndAppHandler, tashVerifyAndSetHandler, tashSelectorHandler, tashCommunicator) {
        }

        protected override async Task ProcessSingleTaskAsync(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), LogId))) {
                var s = string.IsNullOrEmpty(status.TaskBeingProcessed.ControlName)
                    ? $"Processing a task of type {status.TaskBeingProcessed.Type} in {nameof(TashHandler)}"
                    : $"Processing a task of type {status.TaskBeingProcessed.Type} on {status.TaskBeingProcessed.ControlName} in {nameof(TashHandler)}";
                SimpleLogger.LogInformation(s);

                switch (status.TaskBeingProcessed.Type) {
                    case ControllableProcessTaskType.Reset:
                        await TashCommunicator.CommunicateAndShowCompletedOrFailedAsync(status, false, "");
                        break;
                    default:
                        await base.ProcessSingleTaskAsync(status);
                        break;
                }
            }
        }
    }
}
