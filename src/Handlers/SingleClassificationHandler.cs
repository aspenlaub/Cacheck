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

    protected IList<IPostingClassification> Classifications = new List<IPostingClassification>();

    public async Task UpdateSelectableValuesAsync() {
        var selectables = Classifications.GroupBy(c => c.Classification).Select(c => c.Key)
            .OrderBy(c => c)
            .Select(c => new Selectable { Guid = c, Name = c })
            .ToList();

        if (model.SingleClassification.AreSelectablesIdentical(selectables)) { return; }

        model.SingleClassification.UpdateSelectables(selectables);
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        await dataCollectorGetter().CollectAndShowAsync();
    }

    public async Task UpdateSelectableValuesAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
            IList<IInverseClassificationPair> inverseClassifications) {
        var usedClassifications = postingClassificationsMatcher.MatchingClassifications(postings, classifications)
            .Where(c => !IsInverseClassification(c, inverseClassifications)).ToList();
        Classifications = new List<IPostingClassification>(usedClassifications);
        await UpdateSelectableValuesAsync();
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
        await dataCollectorGetter().CollectAndShowAsync();
    }
}