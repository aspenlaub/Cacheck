using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
public class SingleMonthHandler(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler,
        Func<IDataCollector> dataCollectorGetter) : ISingleMonthHandler {

    public async Task UpdateSelectableValuesAsync(IList<IPosting> postings) {
        if (postings.Count == 0) {
            return;
        }

        DateTime posting = postings.Max(p => p.Date);
        await UpdateSelectableValuesAsync(posting.Month, posting.Year);
    }

    public async Task UpdateSelectableValuesAsync() {
        int year = DateTime.Today.Year;
        int month = DateTime.Today.Month;
        if (month <= 2) {
            month = 10 + month;
            year --;
        } else {
            month -= 2;
        }
        await UpdateSelectableValuesAsync(month, year);
    }

    private async Task UpdateSelectableValuesAsync(int lastPostingsMonth, int lastPostingsYear) {
        int previousYear = lastPostingsYear - 1;
        var selectables = new List<Selectable> {
            new() { Guid = "0", Name = "" },
            CreateSelectable(1, "January", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(2, "February", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(3, "March", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(4, "April", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(5, "May", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(6, "June", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(7, "July", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(8, "August", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(9, "September", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(10, "October", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(11, "November", lastPostingsMonth, lastPostingsYear, previousYear),
            CreateSelectable(12, "December", lastPostingsMonth, lastPostingsYear, previousYear),
        };
        model.SingleMonth.UpdateSelectables(selectables);
        await guiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
    }

    private static Selectable CreateSelectable(int monthNo, string monthName, int lastPostingsMonth, int lastPostingsYear, int previousYear) {
        int year = monthNo <= lastPostingsMonth ? lastPostingsYear : previousYear;
        return new Selectable { Guid = (year * 100 + monthNo).ToString(), Name = monthName + " " + year };
    }

    public async Task SelectedIndexChangedAsync(int selectedIndex) {
        if (model.SingleMonth.SelectedIndex == selectedIndex) { return; }

        model.SingleMonth.SelectedIndex = selectedIndex;
        await dataCollectorGetter().CollectAndShowAsync();
    }
}
