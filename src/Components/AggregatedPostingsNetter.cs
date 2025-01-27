using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class AggregatedPostingsNetter : IAggregatedPostingsNetter {
    public IDictionary<IFormattedClassification, double> Net(IDictionary<IFormattedClassification, double> aggregation,
            IList<IInverseClassificationPair> inverseClassifications, IList<string> classificationsToKeepEvenIfZero) {
        foreach (IInverseClassificationPair inverseClassificationPair in inverseClassifications) {
            string classification = inverseClassificationPair.Classification;
            string classification2 = inverseClassificationPair.InverseClassification;
            var keys = aggregation
                .Where(x => x.Key.Classification == classification || x.Key.Classification == classification2).Select(x => x.Key).ToList();
            foreach (IFormattedClassification key in keys) {
                foreach (IFormattedClassification key2 in keys.Where(x => x.Sign != key.Sign)) {
                    double value = aggregation[key];
                    double value2 = aggregation[key2];
                    if (value == 0 || value2 == 0) { continue; }

                    if (value > value2) {
                        aggregation[key] -= aggregation[key2];
                        aggregation[key2] = 0;
                    } else {
                        aggregation[key2] -= aggregation[key];
                        aggregation[key] = 0;
                    }
                }
            }
        }

        return aggregation.Where(x => x.Value > 0 || classificationsToKeepEvenIfZero.Contains(x.Key.Classification)).ToDictionary(x => x.Key, x => x.Value);
    }
}