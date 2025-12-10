using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class ClassifiedPosting : IClassifiedPosting {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string Classification { get; set; }
    public string Clue { get; set; }
    public string Remark { get; set; }
    public bool IsIndividual { get; set; }
    public string PostingHash { get; set; }
    public bool Ineliminable { get; set; }
    public bool Unfair { get; set; }
    public string FormattedDate => $"{Date.Year}-{Date.Month:D2}-{Date.Day:D2}";
}