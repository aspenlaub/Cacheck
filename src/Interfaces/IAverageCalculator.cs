using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IAverageCalculator {
        Task CalculateAndShowAverageAsync(IContainer container, IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications);
    }
}
