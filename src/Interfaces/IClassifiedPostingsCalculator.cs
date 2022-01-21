using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IClassifiedPostingsCalculator {
        Task CalculateAndShowClassifiedPostingsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications, DateTime minDate, double minAmount);
    }
}
