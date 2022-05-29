using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ICacheckHandlers {
    ISimpleCollectionViewSourceHandler OverallSumsHandler { get; }
    ISimpleCollectionViewSourceHandler ClassificationSumsHandler { get; }
    ISimpleCollectionViewSourceHandler ClassificationAveragesHandler { get; }
    ISimpleCollectionViewSourceHandler MonthlyDeltasHandler { get; }
    ISimpleCollectionViewSourceHandler ClassifiedPostingsHandler { get; }
    ISimpleTextHandler LogTextHandler { get; }
}