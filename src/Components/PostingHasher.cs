using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingHasher : IPostingHasher {
    private static readonly IDictionary<IPosting, string> Cache = new Dictionary<IPosting, string>();

    public string Hash(IPosting posting) {
        if (Cache.TryGetValue(posting, out var hash)) {
            return hash;
        }

        var s = posting.Date.ToString("yyyyMMdd")
                + "9"
                + (posting.Amount >= 0 ? "0" : "9")
                + Math.Ceiling(Math.Abs(posting.Amount));

        Cache[posting] = posting.Remark.ToCharArray()
             .Select(ConvertNonDigits)
             .Where(char.IsDigit)
             .Aggregate(s, (current, c) => current + c);

        return Cache[posting];
    }

    private char ConvertNonDigits(char c) {
        switch (c) {
            case ' ':
                return '0';
            case '.':
                return '1';
            case '/':
                return '2';
            default:
                return c;
        }
    }
}