using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class LiquidityPlanCalculator : ILiquidityPlanCalculator {
    public double Calculate(IFormattedClassification classification, double sumPastTwelveMonths,
            IList<ILiquidityPlanClassification> liquidityPlanClassifications) {
        var liquidityPlanClassification = FindLiquidityPlanClassification(classification, liquidityPlanClassifications);
        if (liquidityPlanClassification == null) { return 0; }

        var result =
            liquidityPlanClassification.Target > 0
                ? liquidityPlanClassification.Target
                : Math.Ceiling(sumPastTwelveMonths * liquidityPlanClassification.Percentage / 100);
        return classification.Sign == "-" ? -result : result;
    }

    private ILiquidityPlanClassification FindLiquidityPlanClassification(IFormattedClassification classification,
            IEnumerable<ILiquidityPlanClassification> liquidityPlanClassifications) {
        return liquidityPlanClassifications.FirstOrDefault(i => i.Classification == classification.Classification);
    }
}