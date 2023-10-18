using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IReservationsCalculator {
    double Calculate(IFormattedClassification classification, double sumPastTwelveMonths,
                     IList<IIrregularDebitClassification> irregularDebitClassifications);
}