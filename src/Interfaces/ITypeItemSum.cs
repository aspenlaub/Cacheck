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
    double SumLastYear { get; set; }
    double SumYearBeforeLast { get; set; }
    double SumTwoYearsBeforeLast { get; set; }
    double SumPast24Months { get; set; }
    double SumLastTwoYears { get; set; }
    double TwoYearSumBeforeLastYear { get; set; }
    double TwoYearSumTwoYearsBeforeLastYear { get; set; }
}