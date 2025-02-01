using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
public class SingleMonthHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
        Func<IDataCollector> dataCollectorGetter) : ISimpleSelectorHandler {

    public async Task UpdateSelectableValuesAsync() {
        var selectables = new List<Selectable> {
            new() { Guid = "0", Name = "" },
            new() { Guid = "1", Name = "January" },
            new() { Guid = "2", Name = "February" },
            new() { Guid = "3", Name = "March" },
            new() { Guid = "4", Name = "April" },
            new() { Guid = "5", Name = "May" },
            new() { Guid = "6", Name = "June" },
            new() { Guid = "7", Name = "July" },
            new() { Guid = "8", Name = "August" },
            new() { Guid = "9", Name = "September" },
            new() { Guid = "10", Name = "October" },
            new() { Guid = "11", Name = "November" },
            new() { Guid = "12", Name = "December" },
        };
        model.SingleMonth.UpdateSelectables(selectables);
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        if (model.SingleMonth.SelectedIndex == selectedIndex) { return; }

        model.SingleMonth.SelectedIndex = selectedIndex;
        await dataCollectorGetter().CollectAndShowAsync();
    }
}
