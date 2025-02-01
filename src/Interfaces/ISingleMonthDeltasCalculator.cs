using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ISingleMonthDeltasCalculator {
    Task CalculateAndShowSingleMonthDeltasAsync(IList<IPosting> allPostings,
        IList<IPostingClassification> postingClassifications, int month);
}
