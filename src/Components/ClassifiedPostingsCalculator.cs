using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class ClassifiedPostingsCalculator : IClassifiedPostingsCalculator {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingClassificationsMatcher _PostingClassificationsMatcher;

    public ClassifiedPostingsCalculator(IDataPresenter dataPresenter, IPostingClassificationsMatcher postingClassificationsMatcher) {
        _DataPresenter = dataPresenter;
        _PostingClassificationsMatcher = postingClassificationsMatcher;
    }

    public async Task CalculateAndShowClassifiedPostingsAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
            DateTime minDate, double minAmount, string singleClassification, string singleClassificationInverse) {
        var classifiedPostings = new List<IClassifiedPosting>();

        foreach (var posting in allPostings) {
            if (!IsPostingRelevantHere(posting, postingClassifications, minDate, minAmount,
                singleClassification, singleClassificationInverse, out var classification)) { continue; }

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

        await _DataPresenter.Handlers.ClassifiedPostingsHandler.CollectionChangedAsync(classifiedPostings.OfType<ICollectionViewSourceEntity>().ToList());
    }

    private bool IsPostingRelevantHere(IPosting posting, IList<IPostingClassification> postingClassifications, DateTime minDate, double minAmount,
            string singleClassification, string singleClassificationInverse, out IPostingClassification classification) {
        classification = null;
        if (Math.Abs(posting.Amount) < minAmount) { return false; }
        if (posting.Date < minDate) { return false; }

        var classifications = _PostingClassificationsMatcher.MatchingClassifications(posting, postingClassifications);
        if (classifications.Count != 1) { return false; }

        classification = classifications[0];
        if (singleClassification != "" || singleClassificationInverse != "") {
            return classification.Classification == singleClassification || classification.Classification == singleClassificationInverse;
        }

        return true;
    }
}