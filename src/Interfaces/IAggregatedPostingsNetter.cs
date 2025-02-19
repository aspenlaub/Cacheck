using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IAggregatedPostingsNetter {
    IDictionary<IFormattedClassification, IAggregatedPosting> Net(IDictionary<IFormattedClassification, IAggregatedPosting> aggregation,
        IList<IInverseClassificationPair> inverseClassifications, IList<string> classificationsToKeepEvenIfZero);
}