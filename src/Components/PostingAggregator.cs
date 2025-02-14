﻿using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter,
                IPostingClassificationsMatcher postingClassificationsMatcher,
                IFormattedClassificationComparer formattedClassificationComparer,
                ICalculationLogger calculationLogger) : IPostingAggregator {

    public IDictionary<IFormattedClassification, double> AggregatePostings(IEnumerable<IPosting> postings,
                IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos) {

        var result = new Dictionary<IFormattedClassification, double>(formattedClassificationComparer);
        var resultDrillDown = new Dictionary<string, IList<IPosting>>();
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
            if (result.TryAdd(formattedClassification, 0)) {
                resultDrillDown[formattedClassificationToString] = new List<IPosting>();
            }

            double amount = classification.IsMonthClassification ? posting.Amount : Math.Abs(posting.Amount);
            calculationLogger.RegisterContribution(formattedClassificationToString, amount, posting);
            result[formattedClassification] += amount;
            resultDrillDown[formattedClassificationToString].Add(posting);
        }
        return result;
    }
}