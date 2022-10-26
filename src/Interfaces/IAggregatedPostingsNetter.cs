using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IAggregatedPostingsNetter {
    IDictionary<IFormattedClassification, double> Net(IDictionary<IFormattedClassification, double> aggregation, IList<IInverseClassificationPair> inverseClassifications);
}