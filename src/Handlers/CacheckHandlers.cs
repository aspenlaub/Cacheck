using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Handlers {
    public class CacheckHandlers : ICacheckHandlers {
        public ISimpleTextHandler ConsoleOutputTextHandler { get; set; }
    }
}
