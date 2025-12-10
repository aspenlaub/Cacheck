using System;
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
    public double SumYearBeforeLast { get; set; }
    public double SumTwoYearsBeforeLast { get; set; }
    public double SumPast24Months { get; set; }
    public double SumLastTwoYears { get; set; }
    public double TwoYearSumBeforeLastYear { get; set; }
    public double TwoYearSumTwoYearsBeforeLastYear { get; set; }

    public override string ToString() {
        return $@"{Item}({Type}): {Math.Round(Sum, 2)}";
    }
}