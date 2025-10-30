using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class TypeItemSum : ITypeItemSum {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Item { get; set; }
    public double Sum { get; set; }
    public double SumPastHalfYear { get; set; }
    public double SumPastTwelveMonths { get; set; }
    public double SumLastYear { get; set; }
}