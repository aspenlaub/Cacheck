using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

public class FakeCacheckHandlers : ICacheckHandlers {
    public List<ITypeItemSum> OverallSums { get; } = [];
    public List<ITypeItemSum> ClassificationSums { get; } = [];
    public List<ITypeItemSum> ClassificationAverages { get; } = [];
    public List<ITypeMonthDelta> MonthlyDeltas { get; } = [];
    public List<ITypeMonthDetails> MonthlyDetails { get; } = [];
    public List<IClassifiedPosting> ClassifiedPostings { get; } = [];

    public ISimpleCollectionViewSourceHandler OverallSumsHandler { get; }
    public ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; }
    public ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; }
    public ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; }
    public ISimpleCollectionViewSourceHandler MonthlyDetailsHandler { get; }
    public ISimpleCollectionViewSourceHandler ClassifiedPostingsHandler { get; }
    public ISimpleTextHandler LogTextHandler { get; }
    public ISingleClassificationHandler SingleClassificationHandler { get; }
    public ISimpleTextHandler LiquidityPlanSumTextHandler { get; }
    public ISimpleTextHandler ReservationsSumTextHandler { get; }
    public ISimpleTextHandler MinimumAmountHandler { get; }
    public ISimpleTextHandler FromDayHandler { get; }
    public ISimpleTextHandler ToDayHandler { get; }

    public FakeCacheckHandlers(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
        OverallSumsHandler = new FakeCacheckHandler<ITypeItemSum>(OverallSums);
        ClassificationSumsHandler = new FakeCacheckHandler<ITypeItemSum>(ClassificationSums);
        ClassificationAveragesHandler = new FakeCacheckHandler<ITypeItemSum>(ClassificationAverages);
        MonthlyDeltasHandler = new FakeCacheckHandler<ITypeMonthDelta>(MonthlyDeltas);
        MonthlyDetailsHandler = new FakeCacheckHandler<ITypeMonthDetails>(MonthlyDetails);
        ClassifiedPostingsHandler = new FakeCacheckHandler<IClassifiedPosting>(ClassifiedPostings);
        LogTextHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.Log);
        SingleClassificationHandler = new FakeCacheckSelectorHandler();
        LiquidityPlanSumTextHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.LiquidityPlanSum);
        ReservationsSumTextHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.ReservationsSum);
        MinimumAmountHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.MinimumAmount);
        FromDayHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.FromDay);
        ToDayHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.ToDay);
    }
}