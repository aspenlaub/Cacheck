using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class FormattedClassificationComparer : IFormattedClassificationComparer {
    public bool Equals(IFormattedClassification x, IFormattedClassification y) {
        if (ReferenceEquals(x, y)) { return true; }
        if (ReferenceEquals(x, null)) { return false; }
        if (ReferenceEquals(y, null)) { return false; }
        if (x.GetType() != y.GetType()) { return false; }

        return x.Sign == y.Sign && x.Classification == y.Classification;
    }

    public int GetHashCode(IFormattedClassification obj) {
        return HashCode.Combine(obj.Sign, obj.Classification);
    }
}