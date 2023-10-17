using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ITypeItemSum : ICollectionViewSourceEntity {
    string Type { get; set; }
    string Item { get; set; }
    double Sum { get; set; }
    double SumPastHalfYear { get; set; }
    double SumPastTwelveMonths { get; set; }
    double SumThisYear { get; set; }
    double SumLastYear { get; set; }
}