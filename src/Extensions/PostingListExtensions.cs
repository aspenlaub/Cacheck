using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

#pragma warning disable IDE0046

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;

public static class PostingListExtensions {
    public static bool AreAllPostingsPreClassified(this IList<IPosting> postings) {
        if (!postings.Any(p => p is IPreClassifiedPosting)) {
            return false;
        }

        return !postings.All(p => p is IPreClassifiedPosting) ? throw new NotSupportedException() : true;
    }
}
