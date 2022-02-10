using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingAggregator : IPostingAggregator {
        private readonly IPostingClassificationFormatter PostingClassificationFormatter;
        private readonly IPostingClassificationMatcher PostingClassificationMatcher;
        private readonly IFormattedClassificationComparer FormattedClassificationComparer;
        private readonly ICalculationLogger CalculationLogger;

        public PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter, IPostingClassificationMatcher postingClassificationMatcher,
                IFormattedClassificationComparer formattedClassificationComparer, ICalculationLogger calculationLogger) {
            PostingClassificationFormatter = postingClassificationFormatter;
            PostingClassificationMatcher = postingClassificationMatcher;
            FormattedClassificationComparer = formattedClassificationComparer;
            CalculationLogger = calculationLogger;
        }

        public IDictionary<IFormattedClassification, double> AggregatePostings(IEnumerable<IPosting> postings,
                IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos) {
            var result = new Dictionary<IFormattedClassification, double>(FormattedClassificationComparer);
            var resultDrillDown = new Dictionary<string, IList<IPosting>>();
            foreach (var posting in postings) {
                var classifications = postingClassifications.Where(c => PostingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
                var combinedClassifications = classifications.Select(c => PostingClassificationFormatter.Format(c).CombinedClassification).Distinct().ToList();
                switch (combinedClassifications.Count) {
                    case 0 when Math.Abs(posting.Amount) <= 70:
                        continue;
                    case 0:
                        errorsAndInfos.Errors.Add($"Amount of {posting.Amount} ('{posting.Remark}') could not be classified");
                        continue;
                    case > 1 when posting.Amount != 0:
                        var combinedClassification0 = PostingClassificationFormatter.Format(classifications[0]).CombinedClassification;
                        var combinedClassification1 = PostingClassificationFormatter.Format(classifications[1]).CombinedClassification;
                        errorsAndInfos.Errors.Add($"Classification of '{posting.Remark}' is ambiguous between '{combinedClassification0}' and '{combinedClassification1}'");
                        break;
                }

                var classification = classifications[0];
                var formattedClassification = PostingClassificationFormatter.Format(classification);
                var formattedClassificationToString = formattedClassification.ToString() ?? "";
                if (!result.ContainsKey(formattedClassification)) {
                    result[formattedClassification] = 0;
                    resultDrillDown[formattedClassificationToString] = new List<IPosting>();
                }

                var amount = classification.IsMonthClassification ? posting.Amount : Math.Abs(posting.Amount);
                CalculationLogger.RegisterContribution(formattedClassificationToString, amount, posting);
                result[formattedClassification] += amount;
                resultDrillDown[formattedClassificationToString].Add(posting);
            }
            return result;
        }
    }
}
