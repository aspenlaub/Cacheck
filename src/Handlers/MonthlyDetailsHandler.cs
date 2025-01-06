using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;

public class MonthlyDetailsHandler(ICacheckApplicationModel model,
        IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) : ISimpleCollectionViewSourceHandler {

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
        model.MonthlyDetails.Items.Clear();
        foreach (var item in items.Where(item => item.GetType() == model.MonthlyDetails.EntityType)) {
            model.MonthlyDetails.Items.Add(item);
        }
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        var list = JsonSerializer.Deserialize<List<TypeMonthDetails>>(json);
        return list?.OfType<ICollectionViewSourceEntity>().ToList() ?? [];
    }
}