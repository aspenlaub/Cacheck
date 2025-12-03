using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class ClassifiedPostingsCalculator(IDataPresenter dataPresenter,
                IPostingClassificationsMatcher postingClassificationsMatcher) : IClassifiedPostingsCalculator {

    public async Task<IList<IClassifiedPosting>> CalculateAndShowClassifiedPostingsAsync(IList<IPosting> allPostings,
                IList<IPostingClassification> postingClassifications,
                DateTime minDate, double minAmount,
                string singleClassification, string singleClassificationInverse) {

        if (allPostings.AreAllPostingsPreClassified()) {
            throw new NotImplementedException("Pre-classified postings cannot yet be used here");
        }

        IList<IClassifiedPosting> classifiedPostings = CalculateClassifiedPostings(allPostings, postingClassifications, minDate, minAmount,
                                                                                   singleClassification, singleClassificationInverse);
        await dataPresenter.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(classifiedPostings.OfType<ICollectionViewSourceEntity>().ToList());
        return classifiedPostings;
    }

    public IList<IClassifiedPosting> CalculateClassifiedPostings(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                DateTime minDate, double minAmount, string singleClassification, string singleClassificationInverse) {

        if (allPostings.AreAllPostingsPreClassified()) {
            throw new NotImplementedException("Pre-classified postings cannot yet be used here");
        }

        var classifiedPostings = new List<IClassifiedPosting>();

        foreach (IPosting posting in allPostings) {
            if (!IsPostingRelevantHere(posting, postingClassifications, minDate, minAmount,
                                       singleClassification, singleClassificationInverse, out IPostingClassification classification)) { continue; }

            var classifiedPosting = new ClassifiedPosting {
                Date = posting.Date,
                Amount = posting.Amount,
                Classification = classification.Classification,
                Clue = classification.Clue,
                Remark = posting.Remark,
                IsIndividual = classification.IsIndividual,
                PostingHash = classification.PostingHash,
                Ineliminable = classification.Ineliminable
            };
            classifiedPostings.Add(classifiedPosting);
        }

        classifiedPostings = classifiedPostings.OrderByDescending(cp => cp.Date).ToList();

        return classifiedPostings;
    }

    private bool IsPostingRelevantHere(IPosting posting, IList<IPostingClassification> postingClassifications, DateTime minDate, double minAmount,
                                       string singleClassification, string singleClassificationInverse, out IPostingClassification classification) {
        classification = null;
        if (Math.Abs(posting.Amount) < minAmount) { return false; }
        if (posting.Date < minDate) { return false; }

        IList<IPostingClassification> classifications = postingClassificationsMatcher.MatchingClassifications(posting, postingClassifications);
        if (!AreClassificationsEquivalent(classifications)) { return false; }

        classification = classifications[0];
        return singleClassification == "" && singleClassificationInverse == ""
               || classification.Classification == singleClassification
               || classification.Classification == singleClassificationInverse;
    }

    private bool AreClassificationsEquivalent(IList<IPostingClassification> classifications) {
        if (!classifications.Any()) { return false; }
        if (classifications.Count == 1) { return true;  }

        for (int i = 1; i < classifications.Count; i++) {
            if (classifications[0].Classification != classifications[i].Classification) { return false; }
            if (classifications[0].Credit != classifications[i].Credit) { return false; }
        }

        return true;
    }
}