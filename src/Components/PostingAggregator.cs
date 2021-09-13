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
        // private readonly ISpecialClueMatcher SpecialClueMatcher;

        public PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter, IPostingClassificationMatcher postingClassificationMatcher,
            // ReSharper disable once UnusedParameter.Local
            IFormattedClassificationComparer formattedClassificationComparer, ISpecialClueMatcher specialClueMatcher) {
            PostingClassificationFormatter = postingClassificationFormatter;
            PostingClassificationMatcher = postingClassificationMatcher;
            FormattedClassificationComparer = formattedClassificationComparer;
            // SpecialClueMatcher = specialClueMatcher;
        }

        public IDictionary<IFormattedClassification, double> AggregatePostings(IEnumerable<IPosting> postings,
                IList<IPostingClassification> postingClassifications, IList<ISpecialClue> specialClues, IErrorsAndInfos errorsAndInfos) {
            var result = new Dictionary<IFormattedClassification, double>(FormattedClassificationComparer);
            foreach (var posting in postings) {
                var classifications = postingClassifications.Where(c => PostingClassificationMatcher.DoesPostingMatchClassification(posting, c)).ToList();
                switch (classifications.Count) {
                    case 0 when Math.Abs(posting.Amount) <= 250:
                        continue;
                    case 0:
                        errorsAndInfos.Infos.Add($"Amount of {posting.Amount} ('{posting.Remark}') could not be classified");
                        continue;
                    case > 1 when posting.Amount != 0:
                        var combinedClassification0 = PostingClassificationFormatter.Format(classifications[0]).CombinedClassification;
                        var combinedClassification1 = PostingClassificationFormatter.Format(classifications[1]).CombinedClassification;
                        errorsAndInfos.Errors.Add($"Classification of '{posting.Remark}' is ambiguous between '{combinedClassification0}' and '{combinedClassification1}'");
                        break;
                }

                var classification = classifications[0];
                var formattedClassification = PostingClassificationFormatter.Format(classification);
                if (!result.ContainsKey(formattedClassification)) {
                    result[formattedClassification] = 0;
                }

                result[formattedClassification] += classification.IgnoreCredit ? posting.Amount : Math.Abs(posting.Amount);
            }
            return result;
        }
    }
}
