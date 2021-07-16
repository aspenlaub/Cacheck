using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.GUI {
    public class CacheckGuiToApplicationGate : GuiToApplicationGateBase<CacheckApplication> {
        public CacheckGuiToApplicationGate(IBusy busy, CacheckApplication application) : base(busy, application) {
        }
    }
}
