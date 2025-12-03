using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SummaryCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
        IAggregatedPostingsNetter aggregatedPostingsNetter,
        IPostingClassificationsMatcher postingClassificationsMatcher) : ISummaryCalculator {

    public async Task<bool> CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                                IList<IInverseClassificationPair> inverseClassifications) {
        if (allPostings.AreAllPostingsPreClassified()) {
            throw new NotImplementedException("Pre-classified postings cannot yet be used here");
        }

        var errorsAndInfos = new ErrorsAndInfos();
        var fairPostings = postingClassificationsMatcher
            .MatchingPostings(allPostings, postingClassifications, c => c?.Unfair != true)
            .ToList();
        IDictionary<IFormattedClassification, IAggregatedPosting> pureDebitCreditAggregation = postingAggregator.AggregatePostings(fairPostings, [
            new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
            new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
        ], errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return false;
        }

        var overallSumList = pureDebitCreditAggregation.Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value.Sum }
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.OverallSumsHandler.CollectionChangedAsync(overallSumList);

        errorsAndInfos = new ErrorsAndInfos();
        IDictionary<IFormattedClassification, IAggregatedPosting> detailedAggregation = postingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await dataPresenter.WriteErrorsAsync(errorsAndInfos);
            return false;
        }

        detailedAggregation = aggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications, []);

        if (!detailedAggregation.Any()) {
            return true;
        }

        var classificationSumList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value.Sum }
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await dataPresenter.Handlers.ClassificationSumsHandler.CollectionChangedAsync(classificationSumList);
        return true;
    }
}