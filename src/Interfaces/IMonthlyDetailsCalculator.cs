using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IMonthlyDetailsCalculator {
    Task CalculateAndShowMonthlyDetailsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications);
}