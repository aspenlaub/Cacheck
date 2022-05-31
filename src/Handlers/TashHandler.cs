using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Enums;
using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class TashHandler : TashHandlerBase<CacheckApplicationModel> {
    public TashHandler(ITashAccessor tashAccessor, ISimpleLogger simpleLogger, IButtonNameToCommandMapper buttonNameToCommandMapper,
                IToggleButtonNameToHandlerMapper toggleButtonNameToHandlerMapper, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
                ITashVerifyAndSetHandler<ICacheckApplicationModel> tashVerifyAndSetHandler, ITashSelectorHandler<ICacheckApplicationModel> tashSelectorHandler,
                ITashCommunicator<ICacheckApplicationModel> tashCommunicator, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
        : base(tashAccessor, simpleLogger, buttonNameToCommandMapper, toggleButtonNameToHandlerMapper, guiAndAppHandler, tashVerifyAndSetHandler,
                tashSelectorHandler, tashCommunicator, methodNamesFromStackFramesExtractor) {
    }

    protected override async Task ProcessSingleTaskAsync(ITashTaskHandlingStatus<CacheckApplicationModel> status) {
        using (SimpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(TashAccessor), SimpleLogger.LogId))) {
            var methodNamesFromStack = MethodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            var s = string.IsNullOrEmpty(status.TaskBeingProcessed.ControlName)
                ? $"Processing a task of type {status.TaskBeingProcessed.Type} in {nameof(TashHandler)}"
                : $"Processing a task of type {status.TaskBeingProcessed.Type} on {status.TaskBeingProcessed.ControlName} in {nameof(TashHandler)}";
            SimpleLogger.LogInformationWithCallStack(s, methodNamesFromStack);

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