using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter,
                IPostingClassificationsMatcher postingClassificationsMatcher,
                IFormattedClassificationComparer formattedClassificationComparer,
                ICalculationLogger calculationLogger) : IPostingAggregator {

    public IDictionary<IFormattedClassification, IAggregatedPosting> AggregatePostings(IEnumerable<IPosting> postings,
            IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos) {

        var result = new Dictionary<IFormattedClassification, IAggregatedPosting>(formattedClassificationComparer);
        foreach (IPosting posting in postings) {
            var classifications = postingClassificationsMatcher.MatchingClassifications(posting, postingClassifications)
                .ToList();
            var combinedClassifications = classifications.Select(c => postingClassificationFormatter.Format(c).CombinedClassification).Distinct().ToList();
            switch (combinedClassifications.Count) {
                case 0:
                    errorsAndInfos.Errors.Add($"Amount of {posting.Amount} ('{posting.Remark}') could not be classified");
                    continue;
                case > 1 when posting.Amount != 0:
                    string combinedClassification0 = postingClassificationFormatter.Format(classifications[0]).CombinedClassification;
                    string combinedClassification1 = postingClassificationFormatter.Format(classifications[1]).CombinedClassification;
                    errorsAndInfos.Errors.Add($"Classification of '{posting.Remark}' is ambiguous between '{combinedClassification0}' and '{combinedClassification1}'");
                    break;
            }

            IPostingClassification classification = classifications[0];
            IFormattedClassification formattedClassification = postingClassificationFormatter.Format(classification);
            string formattedClassificationToString = formattedClassification.ToString() ?? "";
            result.TryAdd(formattedClassification, new AggregatedPosting());

            double amount = classification.IsMonthClassification ? posting.Amount : Math.Abs(posting.Amount);
            calculationLogger.RegisterContribution(formattedClassificationToString, amount, posting);
            result[formattedClassification].Sum += amount;
            result[formattedClassification].Postings.Add(posting);
        }
        return result;
    }
}