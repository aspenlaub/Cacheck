﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IAverageCalculator {
    Task CalculateAndShowAverageAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
        IList<IInverseClassificationPair> inverseClassifications, IList<ILiquidityPlanClassification> liquidityPlanClassifications,
        IList<IIrregularDebitClassification> irregularDebitClassifications);
}