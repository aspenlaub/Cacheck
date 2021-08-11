﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Newtonsoft.Json;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class ClassificationAveragesHandler : ISimpleCollectionViewSourceHandler {
        private readonly ICacheckApplicationModel vModel;
        private readonly IGuiAndAppHandler vGuiAndAppHandler;

        public ClassificationAveragesHandler(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler) {
            vModel = model;
            vGuiAndAppHandler = guiAndAppHandler;
        }

        public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
            vModel.ClassificationAverages.Items.Clear();
            foreach (var item in items.Where(item => item.GetType() == vModel.ClassificationAverages.EntityType)) {
                vModel.ClassificationAverages.Items.Add(item);
            }
            await vGuiAndAppHandler.EnableOrDisableButtonsThenSyncGuiAndAppAsync();
        }

        public IList<ICollectionViewSourceEntity> DeserializeJsonObject(string text) {
            var list = JsonConvert.DeserializeObject<List<TypeItemSum>>(text);
            return list == null ? new List<ICollectionViewSourceEntity>() : list.Cast<ICollectionViewSourceEntity>().ToList();
        }
    }
}