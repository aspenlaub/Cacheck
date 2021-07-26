using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingAggregator : IPostingAggregator {
        private readonly IPostingClassificationFormatter vPostingClassificationFormatter;
        private readonly IPostingClassificationMatcher vPostingClassificationMatcher;

        public PostingAggregator(IPostingClassificationFormatter postingClassificationFormatter, IPostingClassificationMatcher postingClassificationMatcher) {
            vPostingClassificationFormatter = postingClassificationFormatter;
            vPostingClassificationMatcher = postingClassificationMatcher;
        }

        public IDictionary<string, double> AggregatePostings(IList<IPosting> postings, IList<IPostingClassification> postingClassifications, IErrorsAndInfos errorsAndInfos) {
            var result = new Dictionary<string, double>();
            foreach (var posting in postings) {
                var classifications = postingClassifications.Where(c
                    => vPostingClassificationMatcher.DoesPostingMatchClassification(posting, c) && (c.IgnoreCredit || c.Credit && posting.Amount > 0 || !c.Credit && posting.Amount < 0)
                    ).ToList();
                switch (classifications.Count) {
                    case 0 when Math.Abs(posting.Amount) <= 250:
                        continue;
                    case 0:
                        errorsAndInfos.Infos.Add($"Amount of {posting.Amount} ('{posting.Remark}') could not be classified");
                        continue;
                    case > 1:
                        errorsAndInfos.Errors.Add($"Classification of '{posting.Remark}' is ambiguous between '{classifications[0].Clue}' and '{classifications[1].Clue}'");
                        break;
                }

                var classification = classifications[0];
                var formattedClassification = vPostingClassificationFormatter.Format(classification);
                if (!result.ContainsKey(formattedClassification)) {
                    result[formattedClassification] = 0;
                }

                result[formattedClassification] = result[formattedClassification] + (classification.IgnoreCredit ? posting.Amount : Math.Abs(posting.Amount));
            }
            return result;
        }
    }
}
