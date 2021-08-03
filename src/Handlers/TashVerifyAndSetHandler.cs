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
                { nameof(status.Model.Log), status.Model.Log },
                { nameof(status.Model.Status), status.Model.Status }
            };
        }

        protected override Dictionary<string, ISimpleTextHandler> TextBoxNamesToTextHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            return new() {
                { nameof(status.Model.Log), vCacheckHandlers.LogTextHandler },
            };
        }

        protected override Dictionary<string, ISimpleCollectionViewSourceHandler> CollectionViewSourceNamesToCollectionViewSourceHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            return new() {
                { nameof(status.Model.OverallSums), vCacheckHandlers.OverallSumsHandler},
                { nameof(status.Model.ClassificationSums), vCacheckHandlers.ClassificationSumsHandler},
                { nameof(status.Model.ClassificationAverages), vCacheckHandlers.ClassificationAveragesHandler},
                { nameof(status.Model.MonthlyDeltas), vCacheckHandlers.MonthlyDeltasHandler},
            };
        }

        protected override Dictionary<string, ICollectionViewSource> CollectionViewSourceNamesToCollectionViewSourceDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
            return new() {
                { nameof(status.Model.OverallSums), status.Model.OverallSums },
                { nameof(status.Model.ClassificationSums), status.Model.ClassificationSums },
                { nameof(status.Model.ClassificationAverages), status.Model.ClassificationAverages },
                { nameof(status.Model.MonthlyDeltas), status.Model.MonthlyDeltas },
            };
        }
    }
}
