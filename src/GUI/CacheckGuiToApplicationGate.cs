using Aspenlaub.Net.GitHub.CSharp.Cacheck.Application;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI;

public class CacheckGuiToApplicationGate(IBusy busy, CacheckApplication application)
                : GuiToApplicationGateBase<CacheckApplication, CacheckApplicationModel>(busy, application);