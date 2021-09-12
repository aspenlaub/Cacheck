﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Newtonsoft.Json;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class PostingAdjustmentsHandler : ISimpleCollectionViewSourceHandler {
        private readonly ICacheckApplicationModel Model;
        private readonly IGuiAndAppHandler GuiAndAppHandler;

        public PostingAdjustmentsHandler(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler) {
            Model = model;
            GuiAndAppHandler = guiAndAppHandler;
        }

        public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
            Model.PostingAdjustments.Items.Clear();
            foreach (var item in items.Where(item => item.GetType() == Model.PostingAdjustments.EntityType)) {
                Model.PostingAdjustments.Items.Add(item);
            }
            await GuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        }

        public IList<ICollectionViewSourceEntity> DeserializeJsonObject(string text) {
            var list = JsonConvert.DeserializeObject<List<TypeItemSum>>(text);
            return list == null ? new List<ICollectionViewSourceEntity>() : list.Cast<ICollectionViewSourceEntity>().ToList();
        }
    }
}
