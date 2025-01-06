using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class ClassificationAveragesHandler(ICacheckApplicationModel model,
        IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) : ISimpleCollectionViewSourceHandler {

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        model.ClassificationAverages.Items.Clear();
        foreach (var item in items.Where(item => item.GetType() == model.ClassificationAverages.EntityType)) {
            model.ClassificationAverages.Items.Add(item);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        var list = JsonSerializer.Deserialize<List<TypeItemSum>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? [];
    }
}