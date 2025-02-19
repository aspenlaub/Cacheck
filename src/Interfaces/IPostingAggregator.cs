using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingAggregator {
    IDictionary<IFormattedClassification, IAggregatedPosting> AggregatePostings(IEnumerable<IPosting> postings,
        IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos);
}