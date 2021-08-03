using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeCacheckHandlers : ICacheckHandlers {
        public List<ITypeItemSum> OverallSums { get; } = new();
        public List<ITypeItemSum> ClassificationSums { get; } = new();
        public List<ITypeItemSum> ClassificationAverages { get; } = new();
        public List<ITypeMonthDelta> MonthlyDeltas { get; } = new();

        public ISimpleCollectionViewSourceHandler OverallSumsHandler { get; }
        public ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; }
        public ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; }
        public ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; }
        public ISimpleTextHandler LogTextHandler { get; }

        public FakeCacheckHandlers(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler) {
            OverallSumsHandler = new FakeCacheckHandler<ITypeItemSum>(OverallSums);
            ClassificationSumsHandler = new FakeCacheckHandler<ITypeItemSum>(ClassificationSums);
            ClassificationAveragesHandler = new FakeCacheckHandler<ITypeItemSum>(ClassificationAverages);
            MonthlyDeltasHandler = new FakeCacheckHandler<ITypeMonthDelta>(MonthlyDeltas);
            LogTextHandler = new CacheckTextHandler(model, guiAndAppHandler, m => m.Log);
        }
    }
}
