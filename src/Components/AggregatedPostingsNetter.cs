using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
// ReSharper disable LoopCanBePartlyConvertedToQuery

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class AggregatedPostingsNetter : IAggregatedPostingsNetter {
    public IDictionary<IFormattedClassification, IAggregatedPosting> Net(IDictionary<IFormattedClassification, IAggregatedPosting> aggregation,
            IList<IInverseClassificationPair> inverseClassifications, IList<string> classificationsToKeepEvenIfZero) {
        foreach (IInverseClassificationPair inverseClassificationPair in inverseClassifications) {
            string classification = inverseClassificationPair.Classification;
            string classification2 = inverseClassificationPair.InverseClassification;
            var keys = aggregation
                .Where(x => x.Key.Classification == classification || x.Key.Classification == classification2).Select(x => x.Key).ToList();
            foreach (IFormattedClassification key in keys) {
                foreach (IFormattedClassification key2 in keys.Where(x => x.Sign != key.Sign)) {
                    double value = aggregation[key].Sum;
                    double value2 = aggregation[key2].Sum;
                    if (value == 0 || value2 == 0) { continue; }

                    if (value > value2) {
                        aggregation[key].Sum -= aggregation[key2].Sum;
                        aggregation[key].Postings.AddRange(aggregation[key2].Postings);
                        aggregation[key2].Sum = 0;
                        aggregation[key2].Postings.Clear();
                    } else {
                        aggregation[key2].Sum -= aggregation[key].Sum;
                        aggregation[key2].Postings.AddRange(aggregation[key].Postings);
                        aggregation[key].Sum = 0;
                        aggregation[key].Postings.Clear();
                    }
                }
            }
        }

        return aggregation.Where(x => x.Value.Sum > 0 || classificationsToKeepEvenIfZero.Contains(x.Key.Classification)).ToDictionary(x => x.Key, x => x.Value);
    }
}