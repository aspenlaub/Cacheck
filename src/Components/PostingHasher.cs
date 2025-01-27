using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingHasher : IPostingHasher {
    private static readonly IDictionary<IPosting, string> _cache = new Dictionary<IPosting, string>();

    public string Hash(IPosting posting) {
        if (_cache.TryGetValue(posting, out string hash)) {
            return hash;
        }

        string s = posting.Date.ToString("yyyyMMdd")
                   + "9"
                   + (posting.Amount >= 0 ? "0" : "9")
                   + Math.Ceiling(Math.Abs(posting.Amount));

        _cache[posting] = posting.Remark.ToCharArray()
             .Select(ConvertNonDigits)
             .Where(char.IsDigit)
             .Aggregate(s, (current, c) => current + c);

        return _cache[posting];
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