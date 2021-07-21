using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class TashVerifyAndSetHandler : TashVerifyAndSetHandlerBase<ICacheckApplicationModel> {
        private readonly ICacheckHandlers vCacheckHandlers;

        public TashVerifyAndSetHandler(ICacheckHandlers cudotosiHandlers, ISimpleLogger simpleLogger, ITashSelectorHandler<ICacheckApplicationModel> tashSelectorHandler, ITashCommunicator<ICacheckApplicationModel> tashCommunicator,
            Dictionary<string, ISelector> selectors) : base(simpleLogger, tashSelectorHandler, tashCommunicator, selectors) {
            vCacheckHandlers = cudotosiHandlers;
        }

        protected override void OnValueTaskProcessed(ITashTaskHandlingStatus<ICacheckApplicationModel> status, bool verify, bool set, string actualValue) { }

        protected override Dictionary<string, ITextBox> TextBoxNamesToTextBoxDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            return new() {
                { nameof(status.Model.ConsoleOutput), status.Model.ConsoleOutput },
                { nameof(status.Model.Status), status.Model.Status }
            };
        }

        protected override Dictionary<string, ISimpleTextHandler> TextBoxNamesToTextHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            return new() {
                { nameof(status.Model.ConsoleOutput), vCacheckHandlers.ConsoleOutputTextHandler }
            };
        }
    }
}
