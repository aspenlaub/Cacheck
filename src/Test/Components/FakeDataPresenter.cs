using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

public class FakeDataPresenter : IDataPresenter {
    private readonly FakeCacheckHandlers _FakeCacheckHandlers;

    public ICacheckHandlers Handlers => _FakeCacheckHandlers;
    public bool Enabled => false;

    public List<ITypeItemSum> OverallSums => _FakeCacheckHandlers.OverallSums;
    public List<ITypeItemSum> ClassificationSums => _FakeCacheckHandlers.ClassificationSums;
    public List<ITypeItemSum> ClassificationAverages => _FakeCacheckHandlers.ClassificationAverages;
    public List<ITypeMonthDelta> MonthlyDeltas => _FakeCacheckHandlers.MonthlyDeltas;
    public List<IClassifiedPosting> ClassifiedPostings => _FakeCacheckHandlers.ClassifiedPostings;

    public FakeDataPresenter(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
        _FakeCacheckHandlers = new FakeCacheckHandlers(model, guiAndAppHandler);
    }

    public string GetLogText() => "";

    public async Task OnSumsChanged(double liquidityPlanSum, double reservationsSum) {
        await Task.CompletedTask;
    }

    public void SetDataCollector(IDataCollector dataCollector) {
    }

    public string SingleClassification() => "";

    public async Task OnClassificationsFoundAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
                                                  IList<IInverseClassificationPair> inverseClassifications) {
        await Task.CompletedTask;
    }
}