using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class TypeSingleMonthDelta : ITypeSingleMonthDelta {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Item { get; set; }
    public double CurrentYear { get; set; }
    public double YearBefore { get; set; }
    public double TwoYearsBefore { get; set; }
}
