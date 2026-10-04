using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class SingleClassificationHandler(ICacheckApplicationModel model,
                IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
                Func<IDataCollector> dataCollectorGetter,
                IPostingClassificationsMatcher postingClassificationsMatcher)
                    : ISingleClassificationHandler {

    protected IList<IPostingClassification> Classifications = [];

    public async Task UpdateSelectableValuesAsync() {
        await UpdateSelectableValuesAsync(false);
    }

    public async Task UpdateSelectableValuesAsync(bool areWeCollecting) {
        bool sync = UpdateSelectableClassificationValues()
            || await UpdateSelectableClassificationPeriodValuesAsync();
        if (!sync) { return; }

        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        if (areWeCollecting) { return; }

        await CollectAndShowAsync();
    }

    private async Task CollectAndShowAsync() {
        IDataCollector theDataCollectorGetter = dataCollectorGetter();
        if (theDataCollectorGetter == null) { return; }

        await theDataCollectorGetter.CollectAndShowAsync();
    }

    private bool UpdateSelectableClassificationValues() {
        var selectables = Classifications.GroupBy(c => c.Classification).Select(c => c.Key)
             .OrderBy(c => c)
             .Select(c => new Selectable { Guid = c, Name = c })
             .ToList();

        if (model.SingleClassification.AreSelectablesIdentical(selectables)) { return false; }

        model.SingleClassification.UpdateSelectables(selectables);
        return true;
    }

    private async Task<bool> UpdateSelectableClassificationPeriodValuesAsync() {
        List<Selectable> selectables = [
                new Selectable { Guid = "OneYear", Name = "One year" },
                new Selectable { Guid = "FewYears", Name = "A few years" }
        ];

        if (model.SingleClassificationPeriod.AreSelectablesIdentical(selectables)) { return false; }

        model.SingleClassificationPeriod.UpdateSelectables(selectables);
        await SelectedPeriodIndexChangedAsync(0);
        return true;
    }

    public async Task UpdateSelectableValuesAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
                                                  IList<IInverseClassificationPair> inverseClassifications, bool areWeCollecting) {
        var usedClassifications = postingClassificationsMatcher.MatchingClassifications(postings, classifications)
            .Where(c => !IsInverseClassification(c, inverseClassifications)).ToList();
        Classifications = new List<IPostingClassification>(usedClassifications);
        await UpdateSelectableValuesAsync(areWeCollecting);
    }

    private bool IsInverseClassification(IPostingClassification c, IEnumerable<IInverseClassificationPair> inverseClassifications) {
        return inverseClassifications.Any(ic
            => ic.Classification == c.Classification && ic.Classification.Length > ic.InverseClassification.Length
               || ic.InverseClassification == c.Classification && ic.Classification.Length < ic.InverseClassification.Length
        );
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        if (model.SingleClassification.SelectedIndex == selectedIndex) { return; }

        model.SingleClassification.SelectedIndex = selectedIndex;
        await CollectAndShowAsync();
    }

    public async Task SelectedPeriodIndexChangedAsync(int selectedIndex) {
        if (model.SingleClassificationPeriod.SelectedIndex == selectedIndex) { return; }

        model.SingleClassificationPeriod.SelectedIndex = selectedIndex;
        await CollectAndShowAsync();
    }
}