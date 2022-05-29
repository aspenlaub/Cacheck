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
    private readonly IDataPresenter DataPresenter;
    private readonly IPostingAggregator PostingAggregator;
    private readonly IPostingClassificationMatcher PostingClassificationMatcher;

    public SummaryCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator, IPostingClassificationMatcher postingClassificationMatcher) {
        DataPresenter = dataPresenter;
        PostingAggregator = postingAggregator;
        PostingClassificationMatcher = postingClassificationMatcher;
    }

    public async Task CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();
        var fairPostings = allPostings.Where(p => postingClassifications.FirstOrDefault(c => PostingClassificationMatcher.DoesPostingMatchClassification(p, c))?.Unfair != true).ToList();
        var pureDebitCreditAggregation = PostingAggregator.AggregatePostings(fairPostings, new List<IPostingClassification> {
            new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
            new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
        }, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        var overallSumList = pureDebitCreditAggregation.Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
        ).Cast<ICollectionViewSourceEntity>().ToList();
        await DataPresenter.Handlers.OverallSumsHandler.CollectionChangedAsync(overallSumList);

        errorsAndInfos = new ErrorsAndInfos();
        var detailedAggregation = PostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos).OrderBy(result => result.Key.CombinedClassification).ToList();
        if (errorsAndInfos.AnyErrors()) {
            await DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return;
        }

        if (detailedAggregation.Any()) {
            var classificationSumList = detailedAggregation.Select(
                result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
            ).Cast<ICollectionViewSourceEntity>().ToList();
            await DataPresenter.Handlers.ClassificationSumsHandler.CollectionChangedAsync(classificationSumList);
        }
    }
}