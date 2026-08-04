using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class Posting : IPosting {
    public string Guid { get; init; } = System.Guid.NewGuid().ToString();
    public DateTime Date { get; init; }
    public double Amount { get; init; }

    public string Remark {
        get;
        set {
            if (value == null || field != null && !value.Contains(field, StringComparison.Ordinal)) {
                return;
            }

            if (string.IsNullOrEmpty(OriginalRemark)) {
                OriginalRemark = value;
            }
            field = value;
        }
    }

    public string OriginalRemark { get; private set; }

    public override string ToString() {
        return $"{Date.ToShortDateString()}, {Amount}, {Remark}";
    }
}