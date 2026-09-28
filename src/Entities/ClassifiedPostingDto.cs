using System;
using System.Security.Cryptography;
using System.Text;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class ClassifiedPostingDto : IPreClassifiedPosting {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string Classification { get; set; }
    public string Sha1 { get; set; }
    public bool IsIndividual { get; set; }
    public bool Ineliminable { get; set; }
    public bool Unfair { get; set; }

    public string Remark {
        get;
        set {
            if (string.IsNullOrEmpty(OriginalRemark)) {
                OriginalRemark = value;
            }

            field = value;
        }
    } = "";
    public string OriginalRemark { get; private set;  } = "";

    public ClassifiedPostingDto() {
    }

    public ClassifiedPostingDto(IClassifiedPosting posting) {
        Guid = posting.Guid;
        Amount = posting.Amount;
        Classification = posting.Classification;
        Date = posting.Date;
        Ineliminable = posting.Ineliminable;
        IsIndividual = posting.IsIndividual;
        Unfair = posting.Unfair;
        Sha1 = Sha1FromRemark(posting.Remark);
    }

    private static string Sha1FromRemark(string clearText) {
        return Convert.ToBase64String(SHA1.HashData(Encoding.UTF8.GetBytes(clearText)));
    }

    public override string ToString() {
        return string.IsNullOrEmpty(Sha1)
            ? $"{Date.ToShortDateString()}, {Amount}, {Classification}"
            : $"{Date.ToShortDateString()}, {Amount}, {Classification}, {Sha1}";
    }
}
