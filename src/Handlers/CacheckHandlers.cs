using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class CacheckHandlers : ICacheckHandlers {
        public ISimpleCollectionViewSourceHandler OverallSumsHandler { get; set; }
        public ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; set; }
        public ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; set; }
        public ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; set; }
        public ISimpleTextHandler LogTextHandler { get; set; }
    }
}
