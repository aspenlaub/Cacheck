using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class ClassifiedPostingDto {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string Classification { get; set; }
    public bool IsIndividual { get; set; }
    public string PostingHash { get; set; }
    public bool Ineliminable { get; set; }

    public ClassifiedPostingDto() {
    }

    public ClassifiedPostingDto(IClassifiedPosting posting) {
        Guid = posting.Guid;
        Amount = posting.Amount;
        Classification = posting.Classification;
        Date = posting.Date;
        Ineliminable = posting.Ineliminable;
        IsIndividual = posting.IsIndividual;
        PostingHash = posting.PostingHash;
    }
}
