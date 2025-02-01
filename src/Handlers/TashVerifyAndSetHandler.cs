using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Handlers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class TashVerifyAndSetHandler(ICacheckHandlers cacheckHandlers, ISimpleLogger simpleLogger,
        ITashSelectorHandler<ICacheckApplicationModel> tashSelectorHandler,
        ITashCommunicator<ICacheckApplicationModel> tashCommunicator,
        Dictionary<string, ISelector> selectors,
        IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
            : TashVerifyAndSetHandlerBase<ICacheckApplicationModel>(simpleLogger, tashSelectorHandler,
                    tashCommunicator, selectors, methodNamesFromStackFramesExtractor) {

    protected override void OnValueTaskProcessed(ITashTaskHandlingStatus<ICacheckApplicationModel> status, bool verify, bool set, string actualValue) { }

    protected override Dictionary<string, ITextBox> TextBoxNamesToTextBoxDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ITextBox> {
            { nameof(status.Model.Log), status.Model.Log },
            { nameof(status.Model.Status), status.Model.Status }
        };
    }

    protected override Dictionary<string, ISimpleTextHandler> TextBoxNamesToTextHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ISimpleTextHandler> {
            { nameof(status.Model.Log), cacheckHandlers.LogTextHandler },
            { nameof(status.Model.LiquidityPlanSum), cacheckHandlers.LiquidityPlanSumTextHandler},
            { nameof(status.Model.ReservationsSum), cacheckHandlers.ReservationsSumTextHandler },
            { nameof(status.Model.MinimumAmount), cacheckHandlers.MinimumAmountHandler},
            { nameof(status.Model.FromDay), cacheckHandlers.FromDayHandler},
            { nameof(status.Model.ToDay), cacheckHandlers.ToDayHandler}
        };
    }

    protected override Dictionary<string, ISimpleCollectionViewSourceHandler> CollectionViewSourceNamesToCollectionViewSourceHandlerDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ISimpleCollectionViewSourceHandler> {
            { nameof(status.Model.OverallSums), cacheckHandlers.OverallSumsHandler},
            { nameof(status.Model.ClassificationSums), cacheckHandlers.ClassificationSumsHandler},
            { nameof(status.Model.ClassificationAverages), cacheckHandlers.ClassificationAveragesHandler},
            { nameof(status.Model.MonthlyDeltas), cacheckHandlers.MonthlyDeltasHandler},
            { nameof(status.Model.MonthlyDetails), cacheckHandlers.MonthlyDetailsHandler},
            { nameof(status.Model.ClassifiedPostings), cacheckHandlers.ClassifiedPostingsHandler},
            { nameof(status.Model.SingleMonthDeltas), cacheckHandlers.SingleMonthDeltasHandler},
        };
    }

    protected override Dictionary<string, ICollectionViewSource> CollectionViewSourceNamesToCollectionViewSourceDictionary(ITashTaskHandlingStatus<ICacheckApplicationModel> status) {
        return new Dictionary<string, ICollectionViewSource> {
            { nameof(status.Model.OverallSums), status.Model.OverallSums },
            { nameof(status.Model.ClassificationSums), status.Model.ClassificationSums },
            { nameof(status.Model.ClassificationAverages), status.Model.ClassificationAverages },
            { nameof(status.Model.MonthlyDeltas), status.Model.MonthlyDeltas },
            { nameof(status.Model.MonthlyDetails), status.Model.MonthlyDetails },
            { nameof(status.Model.ClassifiedPostings), status.Model.ClassifiedPostings },
            { nameof(status.Model.SingleMonthDeltas), status.Model.SingleMonthDeltas },
        };
    }
}