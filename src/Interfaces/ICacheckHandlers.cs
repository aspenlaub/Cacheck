using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ICacheckHandlers {
        ISimpleTextHandler SummaryTextHandler { get; }
        ISimpleTextHandler AverageTextHandler { get; }
        ISimpleTextHandler LogTextHandler { get; }
    }
}
