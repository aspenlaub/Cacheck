using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class ClassificationAveragesHandler : ISimpleCollectionViewSourceHandler {
    private readonly ICacheckApplicationModel _Model;
    private readonly IGuiAndAppHandler<CacheckApplicationModel> _GuiAndAppHandler;

    public ClassificationAveragesHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        _Model.ClassificationAverages.Items.Clear();
        foreach (var item in items.Where(item => item.GetType() == _Model.ClassificationAverages.EntityType)) {
            _Model.ClassificationAverages.Items.Add(item);
        }
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        var list = JsonSerializer.Deserialize<List<TypeItemSum>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? new List<ICollectionViewSourceEntity>();
    }
}