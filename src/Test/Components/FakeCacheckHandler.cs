using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

public class FakeCacheckHandler<T>(List<T> items) : ISimpleCollectionViewSourceHandler
                where T : ICollectionViewSourceEntity {

    public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> changedItems) {
        items.Clear();
        Assert.IsTrue(changedItems.All(item => item is T));
        items.AddRange(changedItems.OfType<T>());
        await Task.CompletedTask;
    }

    public IList<ICollectionViewSourceEntity> DeserializeJson(string json) {
        throw new System.NotImplementedException();
    }
}