using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class ClassifiedPostingsHandler : ISimpleCollectionViewSourceHandler {
    private readonly ICacheckApplicationModel _Model;
    private readonly IGuiAndAppHandler<CacheckApplicationModel> _GuiAndAppHandler;

    public ClassifiedPostingsHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
        _Model = model;
        _GuiAndAppHandler = guiAndAppHandler;
    }

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        _Model.ClassifiedPostings.Items.Clear();
        foreach (var item in items.Where(item => item.GetType() == _Model.ClassifiedPostings.EntityType)) {
            _Model.ClassifiedPostings.Items.Add(item);
        }
        await _GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        var list = JsonSerializer.Deserialize<List<ClassifiedPosting>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? new List<ICollectionViewSourceEntity>();
    }
}