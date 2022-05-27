using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeDataPresenter : IDataPresenter {
        private readonly FakeCacheckHandlers FakeCacheckHandlers;

        public ICacheckHandlers Handlers => FakeCacheckHandlers;

        public List<ITypeItemSum> OverallSums => FakeCacheckHandlers.OverallSums;
        public List<ITypeItemSum> ClassificationSums => FakeCacheckHandlers.ClassificationSums;
        public List<ITypeItemSum> ClassificationAverages => FakeCacheckHandlers.ClassificationAverages;
        public List<ITypeMonthDelta> MonthlyDeltas => FakeCacheckHandlers.MonthlyDeltas;
        public List<IClassifiedPosting> ClassifiedPostings => FakeCacheckHandlers.ClassifiedPostings;

        public FakeDataPresenter(ICacheckApplicationModel model, IGuiAndAppHandler<CacheckApplicationModel> guiAndAppHandler) {
            FakeCacheckHandlers = new FakeCacheckHandlers(model, guiAndAppHandler);
        }

        public string GetLogText() {
            throw new System.NotImplementedException();
        }
    }
}
