using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IDataPresenter {
    ICacheckHandlers Handlers { get; }
    bool Enabled { get; }

    string GetLogText();
    Task OnClassificationsFoundAsync(IList<IPostingClassification> classifications, IList<IPosting> postings,
        IList<IInverseClassificationPair> inverseClassifications, bool areWeCollecting);
    Task OnSumsChanged(double liquidityPlanSum, double reservationsSum);

    void SetDataCollector(IDataCollector dataCollector);
    string SingleClassification();
    double MinimumAmount();
    int FromDay();
    int ToDay();
    int SingleMonthNumber();
}