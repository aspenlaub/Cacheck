using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Newtonsoft.Json;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class OverallSumsHandler : ISimpleCollectionViewSourceHandler {
    private readonly ICacheckApplicationModel _Model;
    private readonly IGuiAndAppHandler<CacheckApplicationModel> _GuiAndAppHandler;

    public OverallSumsHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        _Model.OverallSums.Items.Clear();
        foreach (var item in items.Where(item => item.GetType() == _Model.OverallSums.EntityType)) {
            _Model.OverallSums.Items.Add(item);
        }
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJsonObject(string text) {
        var list = JsonConvert.DeserializeObject<List<TypeItemSum>>(text);
        return list == null ? new List<ICollectionViewSourceEntity>() : list.Cast<ICollectionViewSourceEntity>().ToList();
    }
}