using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class ClassificationSumsHandler(ICacheckApplicationModel model,
                IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) : ISimpleCollectionViewSourceHandler {

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        model.ClassificationSums.Items.Clear();
        foreach (ICollectionViewSourceEntity item in items.Where(item => item.GetType() == model.ClassificationSums.EntityType)) {
            model.ClassificationSums.Items.Add(item);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        List<TypeItemSum> list = JsonSerializer.Deserialize<List<TypeItemSum>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? [];
    }
}