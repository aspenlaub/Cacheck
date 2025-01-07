using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ICacheckHandlers {
    ISimpleCollectionViewSourceHandler OverallSumsHandler { get; }
    ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; }
    ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; }
    ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; }
    ISimpleCollectionViewSourceHandler MonthlyDetailsHandler { get; }
    ISimpleCollectionViewSourceHandler ClassifiedPostingsHandler { get; }
    ISimpleTextHandler LogTextHandler { get; }
    ISingleClassificationHandler SingleClassificationHandler { get; }
    ISimpleTextHandler LiquidityPlanSumTextHandler { get; }
    ISimpleTextHandler ReservationsSumTextHandler { get; }
    ISimpleTextHandler MinimumAmountHandler { get; }
    ISimpleTextHandler FromDayHandler { get; }
    ISimpleTextHandler ToDayHandler { get; }
}