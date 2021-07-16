using Aspenlaub.Net.GitHub.CSharp.Cacheck.App.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App.GUI {
    public class CacheckGuiAndApplicationSynchronizer : GuiAndApplicationSynchronizerBase<CacheckApplicationModel, CacheckWindow> {
        public CacheckGuiAndApplicationSynchronizer(CacheckApplicationModel model, CacheckWindow window) : base(model, window) {
        }
    }
}
