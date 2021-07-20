using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Handlers {
    public class CacheckHandlers : ICacheckHandlers {
        public ISimpleTextHandler ConsoleOutputTextHandler { get; set; }
    }
}
