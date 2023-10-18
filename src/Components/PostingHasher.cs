using System;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingHasher : IPostingHasher {
    public string Hash(IPosting posting) {
        var s = posting.Date.ToString("yyyyMMdd")
                + "9"
                + (posting.Amount >= 0 ? "0" : "9")
                + Math.Ceiling(Math.Abs(posting.Amount));
        return posting.Remark.ToCharArray()
            .Select(ConvertNonDigits)
            .Where(char.IsDigit)
            .Aggregate(s, (current, c) => current + c);
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