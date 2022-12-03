using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class SingleClassificationHandler : ISingleClassificationHandler {
    private readonly ICacheckApplicationModel _Model;
    private readonly IGuiAndAppHandler<CacheckApplicationModel> _GuiAndAppHandler;
    private readonly Func<IDataCollector> _DataCollectorGetter;
    private readonly IPostingClassificationMatcher _PostingClassificationMatcher;

    protected IList<IPostingClassification> Classifications;

    public SingleClassificationHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
            Func<IDataCollector> dataCollectorGetter, IPostingClassificationMatcher postingClassificationMatcher) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
        _DataCollectorGetter = dataCollectorGetter;
        _PostingClassificationMatcher = postingClassificationMatcher;
        Classifications = new List<IPostingClassification>();
    }

    public async Task UpdateSelectableValuesAsync() {
        var selectables = Classifications.GroupBy(c => c.Classification).Select(c => c.Key)
            .OrderBy(c => c)
            .Select(c => new Selectable { Guid = c, Name = c })
            .ToList();

        if (_Model.SingleClassification.AreSelectablesIdentical(selectables)) { return; }

        _Model.SingleClassification.UpdateSelectables(selectables);
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();

        await _DataCollectorGetter().CollectAndShowAsync();
    }

    public async Task UpdateSelectableValuesAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
            IList<IInverseClassificationPair> inverseClassifications) {
        var usedClassifications = classifications
            .Where(c => postings.Any(p => _PostingClassificationMatcher.DoesPostingMatchClassification(p, c)))
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
        if (_Model.SingleClassification.SelectedIndex == selectedIndex) { return; }

        _Model.SingleClassification.SelectedIndex = selectedIndex;
        await _DataCollectorGetter().CollectAndShowAsync();
    }
}