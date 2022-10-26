using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class TashVerifyAndSetHandler : TashVerifyAndSetHandlerBase<ICacheckApplicationModel> {
    private readonly ICacheckHandlers _CacheckHandlers;

    public TashVerifyAndSetHandler(ICacheckHandlers cudotosiHandlers, ISimpleLogger simpleLogger, ITashSelectorHandler<ICacheckApplicationModel> tashSelectorHandler, ITashCommunicator<ICacheckApplicationModel> tashCommunicator,
        Dictionary<string, ISelector> selectors, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : base(simpleLogger, tashSelectorHandler, tashCommunicator, selectors, methodNamesFromStackFramesExtractor) {
        _CacheckHandlers = cudotosiHandlers;
    }

    protected override void OnValueTaskProcessed(ITashTaskHandlingStatus<ICacheckApplicationModel> status, bool verify, bool set, string actualValue) { }

    protected override Dictionary<string, ITextBox> TextBoxNamesToTextBoxDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ITextBox> {
            { nameof(status.Model.Log), status.Model.Log },
            { nameof(status.Model.Status), status.Model.Status }
        };
    }

    protected override Dictionary<string, ISimpleTextHandler> TextBoxNamesToTextHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ISimpleTextHandler> {
            { nameof(status.Model.Log), _CacheckHandlers.LogTextHandler }
        };
    }

    protected override Dictionary<string, ISimpleCollectionViewSourceHandler> CollectionViewSourceNamesToCollectionViewSourceHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ISimpleCollectionViewSourceHandler> {
            { nameof(status.Model.OverallSums), _CacheckHandlers.OverallSumsHandler},
            { nameof(status.Model.ClassificationSums), _CacheckHandlers.ClassificationSumsHandler},
            { nameof(status.Model.ClassificationAverages), _CacheckHandlers.ClassificationAveragesHandler},
            { nameof(status.Model.MonthlyDeltas), _CacheckHandlers.MonthlyDeltasHandler},
            { nameof(status.Model.ClassifiedPostings), _CacheckHandlers.ClassifiedPostingsHandler},
        };
    }

    protected override Dictionary<string, ICollectionViewSource> CollectionViewSourceNamesToCollectionViewSourceDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ICollectionViewSource> {
            { nameof(status.Model.OverallSums), status.Model.OverallSums },
            { nameof(status.Model.ClassificationSums), status.Model.ClassificationSums },
            { nameof(status.Model.ClassificationAverages), status.Model.ClassificationAverages },
            { nameof(status.Model.MonthlyDeltas), status.Model.MonthlyDeltas },
            { nameof(status.Model.ClassifiedPostings), status.Model.ClassifiedPostings },
        };
    }
}