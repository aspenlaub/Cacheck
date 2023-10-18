using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ILiquidityPlanCalculator {
    double Calculate(IFormattedClassification classification, double sumPastTwelveMonths,
        IList<ILiquidityPlanClassification> liquidityPlanClassifications);
}