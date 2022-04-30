using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Vishizhukel.Interfaces.Application;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI {
    public class CacheckGuiAndApplicationSynchronizer : GuiAndApplicationSynchronizerBase<CacheckApplicationModel, CacheckWindow> {
        public CacheckGuiAndApplicationSynchronizer(CacheckApplicationModel model, CacheckWindow window, IApplicationLogger applicationLogger) : base(model, window, applicationLogger) {
        }
    }
}
