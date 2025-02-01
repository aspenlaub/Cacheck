using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ITypeSingleMonthDelta : ICollectionViewSourceEntity {
    string Type { get; set; }
    string Item { get; set; }
    double CurrentYear { get; set; }
    double YearBefore { get; set; }
    double TwoYearsBefore { get; set; }
}
