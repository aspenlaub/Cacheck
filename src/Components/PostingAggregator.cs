using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingAggregator : IPostingAggregator {
    private readonly IPostingClassificationFormatter _PostingClassificationFormatter;
    private readonly IPostingClassificationMatcher _PostingClassificationMatcher;
    private readonly IFormattedClassificationComparer _FormattedClassificationComparer;
    private readonly ICalculationLogger _CalculationLogger;

    public PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter, IPostingClassificationMatcher postingClassificationMatcher,
        IFormattedClassificationComparer formattedClassificationComparer, ICalculationLogger calculationLogger) {
        _PostingClassificationFormatter = postingClassificationFormatter;
        _PostingClassificationMatcher = postingClassificationMatcher;
        _FormattedClassificationComparer = formattedClassificationComparer;
        _CalculationLogger = calculationLogger;
    }

    public IDictionary<IFormattedClassification, double> AggregatePostings(IEnumerable<IPosting> postings,
        IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos) {
        var result = new Dictionary<IFormattedClassification, double>(_FormattedClassificationComparer);
        var resultDrillDown = new Dictionary<string, IList<IPosting>>();
        foreach (var posting in postings) {
            var classifications = postingClassifications.Where(c => _PostingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
            var combinedClassifications = classifications.Select(c => _PostingClassificationFormatter.Format(c).CombinedClassification).Distinct().ToList();
            switch (combinedClassifications.Count) {
                case 0 when Math.Abs(posting.Amount) <= 70:
                    continue;
                case 0:
                    errorsAndInfos.Errors.Add($"Amount of {posting.Amount} ('{posting.Remark}') could not be classified");
                    continue;
                case > 1 when posting.Amount != 0:
                    var combinedClassification0 = _PostingClassificationFormatter.Format(classifications[0]).CombinedClassification;
                    var combinedClassification1 = _PostingClassificationFormatter.Format(classifications[1]).CombinedClassification;
                    errorsAndInfos.Errors.Add($"Classification of '{posting.Remark}' is ambiguous between '{combinedClassification0}' and '{combinedClassification1}'");
                    break;
            }

            var classification = classifications[0];
            var formattedClassification = _PostingClassificationFormatter.Format(classification);
            var formattedClassificationToString = formattedClassification.ToString() ?? "";
            if (!result.ContainsKey(formattedClassification)) {
                result[formattedClassification] = 0;
                resultDrillDown[formattedClassificationToString] = new List<IPosting>();
            }

            var amount = classification.IsMonthClassification ? posting.Amount : Math.Abs(posting.Amount);
            _CalculationLogger.RegisterContribution(formattedClassificationToString, amount, posting);
            result[formattedClassification] += amount;
            resultDrillDown[formattedClassificationToString].Add(posting);
        }
        return result;
    }
}