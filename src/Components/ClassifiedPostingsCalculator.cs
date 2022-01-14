using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class ClassifiedPostingsCalculator : IClassifiedPostingsCalculator {
        private readonly IDataPresenter DataPresenter;
        private readonly IPostingClassificationMatcher PostingClassificationMatcher;

        public ClassifiedPostingsCalculator(IDataPresenter dataPresenter, IPostingClassificationMatcher postingClassificationMatcher) {
            DataPresenter = dataPresenter;
            PostingClassificationMatcher = postingClassificationMatcher;
        }

        public async Task CalculateAndShowClassifiedPostingsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
            var classifiedPostings = new List<IClassifiedPosting>();

            foreach (var posting in allPostings) {
                if (!IsPostingRelevantHere(posting, postingClassifications, out var classification)) { continue; }

                var classifiedPosting = new ClassifiedPosting {
                    Date = posting.Date,
                    Amount = posting.Amount,
                    Classification = classification.Classification,
                    Clue = classification.Clue,
                    Remark = posting.Remark
                };
                classifiedPostings.Add(classifiedPosting);
            }

            classifiedPostings = classifiedPostings.OrderByDescending(cp => cp.Date).ToList();

            await DataPresenter.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(classifiedPostings.Cast<ICollectionViewSourceEntity>().ToList());
        }

        private bool IsPostingRelevantHere(IPosting posting, IEnumerable<IPostingClassification> postingClassifications, out IPostingClassification classification) {
            classification = null;
            if (Math.Abs(posting.Amount) < 250) { return false; }

            classification = postingClassifications.SingleOrDefault(c => PostingClassificationMatcher.DoesPostingMatchClassification(posting, c));
            return classification != null;
        }
    }
}