using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class ReservationsCalculator : IReservationsCalculator {
    public double Calculate(IFormattedClassification classification, double sumPastTwelveMonths,
            IList<IIrregularDebitClassification> irregularDebitClassifications) {
        sumPastTwelveMonths = classification.Sign == "-" ? sumPastTwelveMonths : -sumPastTwelveMonths;
        if (sumPastTwelveMonths <= 0) { return 0; }

        IIrregularDebitClassification irregularDebitClassification = FindIrregularDebitClassification(classification, irregularDebitClassifications);
        if (irregularDebitClassification == null) { return 0; }

        return Math.Ceiling(sumPastTwelveMonths * irregularDebitClassification.Percentage / 100);
    }

    private IIrregularDebitClassification FindIrregularDebitClassification(IFormattedClassification classification,
            IEnumerable<IIrregularDebitClassification> irregularDebitClassifications) {
        return irregularDebitClassifications.FirstOrDefault(i => i.Classification == classification.Classification);
    }
}