﻿using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.GUI;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.GUI {
    public class CacheckGuiAndApplicationSynchronizer : GuiAndApplicationSynchronizerBase<CacheckApplicationModel, CacheckWindow> {
        public CacheckGuiAndApplicationSynchronizer(CacheckApplicationModel model, CacheckWindow window) : base(model, window) {
        }
    }
}