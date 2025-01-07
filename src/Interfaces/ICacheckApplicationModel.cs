using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ICacheckApplicationModel : IApplicationModelBase {
    ICollectionViewSource OverallSums { get; }
    ICollectionViewSource ClassificationSums { get; }
    ICollectionViewSource ClassificationAverages { get; }
    ICollectionViewSource MonthlyDeltas { get; }
    ICollectionViewSource MonthlyDetails { get; }
    ICollectionViewSource ClassifiedPostings { get; }
    ITextBox Log { get; }
    ISelector SingleClassification { get; }
    ITextBox LiquidityPlanSum { get; }
    ITextBox ReservationsSum { get; }
    ITextBox MinimumAmount { get; }
}