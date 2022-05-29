using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;

public class CacheckGuiToApplicationGate : GuiToApplicationGateBase<CacheckApplication, CacheckApplicationModel> {
    public CacheckGuiToApplicationGate(IBusy busy, CacheckApplication application) : base(busy, application) {
    }
}