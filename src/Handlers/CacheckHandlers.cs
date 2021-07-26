using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class CacheckHandlers : ICacheckHandlers {
        public ISimpleTextHandler SummaryTextHandler { get; set; }
        public ISimpleTextHandler AverageTextHandler { get; set; }
        public ISimpleTextHandler MonthlyDeltaTextHandler { get; set; }
        public ISimpleTextHandler LogTextHandler { get; set; }
    }
}
