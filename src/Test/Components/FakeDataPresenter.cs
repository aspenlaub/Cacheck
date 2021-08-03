using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeDataPresenter : IDataPresenter {
        private readonly FakeCacheckHandlers vFakeCacheckHandlers;

        public ICacheckHandlers Handlers => vFakeCacheckHandlers;

        public List<ITypeItemSum> OverallSums => vFakeCacheckHandlers.OverallSums;
        public List<ITypeItemSum> ClassificationSums => vFakeCacheckHandlers.ClassificationSums;
        public List<ITypeItemSum> ClassificationAverages => vFakeCacheckHandlers.ClassificationAverages;
        public List<ITypeMonthDelta> MonthlyDeltas => vFakeCacheckHandlers.MonthlyDeltas;

        public FakeDataPresenter(ICacheckApplicationModel model, IGuiAndAppHandler guiAndAppHandler) {
            vFakeCacheckHandlers = new FakeCacheckHandlers(model, guiAndAppHandler);
        }

        public string GetLogText() {
            throw new System.NotImplementedException();
        }
    }
}
