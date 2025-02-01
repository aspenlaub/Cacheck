using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class CacheckHandlers : ICacheckHandlers {
    public ISimpleCollectionViewSourceHandler OverallSumsHandler { get; init; }
    public ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; init; }
    public ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; init; }
    public ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; init; }
    public ISimpleCollectionViewSourceHandler MonthlyDetailsHandler { get; init; }
    public ISimpleCollectionViewSourceHandler ClassifiedPostingsHandler { get; init; }
    public ISimpleCollectionViewSourceHandler SingleMonthDeltasHandler { get; init; }
    public ISimpleTextHandler LogTextHandler { get; init; }
    public ISingleClassificationHandler SingleClassificationHandler { get; init; }
    public ISingleMonthHandler SingleMonthHandler { get; init; }
    public ISimpleTextHandler LiquidityPlanSumTextHandler { get; init; }
    public ISimpleTextHandler ReservationsSumTextHandler { get; init; }
    public ISimpleTextHandler MinimumAmountHandler { get; init; }
    public ISimpleTextHandler FromDayHandler { get; init; }
    public ISimpleTextHandler ToDayHandler { get; init; }
}