using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeCacheckHandler<T> : ISimpleCollectionViewSourceHandler where T : ICollectionViewSourceEntity {
        private readonly List<T> Items;

        public FakeCacheckHandler(List<T> items) {
            Items = items;
        }

        public async Task CollectionChangedAsync(IList<ICollectionViewSourceEntity> items) {
            Items.Clear();
            Assert.IsTrue(items.All(item => item is T));
            Items.AddRange(items.OfType<T>());
            await Task.CompletedTask;
        }

        public IList<ICollectionViewSourceEntity> DeserializeJsonObject(string text) {
            throw new System.NotImplementedException();
        }
    }
}
