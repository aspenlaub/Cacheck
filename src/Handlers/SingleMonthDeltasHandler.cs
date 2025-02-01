using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class SingleMonthDeltasHandler(ICacheckApplicationModel model,
            IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) : ISimpleCollectionViewSourceHandler {
    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        model.SingleMonthDeltas.Items.Clear();
        foreach (ICollectionViewSourceEntity item in items.Where(item => item.GetType() == model.SingleMonthDeltas.EntityType)) {
            model.SingleMonthDeltas.Items.Add(item);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        List<TypeSingleMonthDelta> list = JsonSerializer.Deserialize<List<TypeSingleMonthDelta>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? [];
    }
}
