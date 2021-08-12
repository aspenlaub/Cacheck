using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IMonthlyDeltaCalculator {
        Task CalculateAndShowMonthlyDeltaAsync(IContainer container, IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications);
    }
}
