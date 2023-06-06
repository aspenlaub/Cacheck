using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ISummaryCalculator {
    Task<bool> CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
        IList<IInverseClassificationPair> inverseClassifications);
}