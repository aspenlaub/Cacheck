using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ICacheckApplicationModel : IApplicationModel {
        ICollectionViewSource OverallSums { get; }
        ICollectionViewSource ClassificationSums { get; }
        ICollectionViewSource ClassificationAverages { get; }
        ICollectionViewSource MonthlyDeltas { get; }
        ICollectionViewSource ClassifiedPostings { get; }
        ITextBox Log { get; }
    }
}
