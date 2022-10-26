using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ISummaryCalculator {
    Task CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
        IList<IInverseClassificationPair> inverseClassifications);
}