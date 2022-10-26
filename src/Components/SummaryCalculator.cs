using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SummaryCalculator : ISummaryCalculator {
    private readonly IDataPresenter _DataPresenter;
    private readonly IPostingAggregator _PostingAggregator;
    private readonly IPostingClassificationMatcher _PostingClassificationMatcher;
    private readonly IAggregatedPostingsNetter _AggregatedPostingsNetter;

    public SummaryCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator, IPostingClassificationMatcher postingClassificationMatcher,
            IAggregatedPostingsNetter aggregatedPostingsNetter) {
        _DataPresenter = dataPresenter;
        _PostingAggregator = postingAggregator;
        _PostingClassificationMatcher = postingClassificationMatcher;
        _AggregatedPostingsNetter = aggregatedPostingsNetter;
    }

    public async Task CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                IList<IInverseClassificationPair> inverseClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();
        var fairPostings = allPostings.Where(p => postingClassifications.FirstOrDefault(c => _PostingClassificationMatcher.DoesPostingMatchClassification(p, c))?.Unfair != true).ToList();
        var pureDebitCreditAggregation = _PostingAggregator.AggregatePostings(fairPostings, new List<IPostingClassification> {
            new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
            new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
        }, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var overallSumList = pureDebitCreditAggregation.Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
        ).Cast<ICollectionViewSourceEntity>().ToList();
        await _DataPresenter.Handlers.OverallSumsHandler.CollectionChangedAsync(overallSumList);

        errorsAndInfos = new ErrorsAndInfos();
        var detailedAggregation = _PostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        detailedAggregation = _AggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications);

        if (detailedAggregation.Any()) {
            var classificationSumList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).Select(
                result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
            ).Cast<ICollectionViewSourceEntity>().ToList();
            await _DataPresenter.Handlers.ClassificationSumsHandler.CollectionChangedAsync(classificationSumList);
        }
    }
}