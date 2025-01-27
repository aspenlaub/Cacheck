using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class MonthlyDeltasHandler(ICacheckApplicationModel model,
                IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) : ISimpleCollectionViewSourceHandler {

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        model.MonthlyDeltas.Items.Clear();
        foreach (ICollectionViewSourceEntity item in items.Where(item => item.GetType() == model.MonthlyDeltas.EntityType)) {
            model.MonthlyDeltas.Items.Add(item);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        List<TypeMonthDelta> list = JsonSerializer.Deserialize<List<TypeMonthDelta>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? [];
    }
}