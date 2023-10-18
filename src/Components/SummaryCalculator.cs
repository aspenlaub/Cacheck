﻿using System.Collections.Generic;
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
    private readonly IPostingClassificationsMatcher _PostingClassificationsMatcher;
    private readonly IAggregatedPostingsNetter _AggregatedPostingsNetter;

    public SummaryCalculator(IDataPresenter dataPresenter, IPostingAggregator postingAggregator,
            IAggregatedPostingsNetter aggregatedPostingsNetter, IPostingClassificationsMatcher postingClassificationsMatcher) {
        _DataPresenter = dataPresenter;
        _PostingAggregator = postingAggregator;
        _AggregatedPostingsNetter = aggregatedPostingsNetter;
        _PostingClassificationsMatcher = postingClassificationsMatcher;
    }

    public async Task<bool> CalculateAndShowSummaryAsync(IList<IPosting> allPostings, IList<IPostingClassification> postingClassifications,
                IList<IInverseClassificationPair> inverseClassifications) {
        var errorsAndInfos = new ErrorsAndInfos();
        var fairPostings = _PostingClassificationsMatcher
            .MatchingPostings(allPostings, postingClassifications, c => c?.Unfair != true)
            .ToList();
        var pureDebitCreditAggregation = _PostingAggregator.AggregatePostings(fairPostings, new List<IPostingClassification> {
            new PostingClassification { Credit = false, Clue = "", Classification = "Debit" },
            new PostingClassification { Credit = true, Clue = "", Classification = "Credit" }
        }, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return false;
        }

        var overallSumList = pureDebitCreditAggregation.Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await _DataPresenter.Handlers.OverallSumsHandler.CollectionChangedAsync(overallSumList);

        errorsAndInfos = new ErrorsAndInfos();
        var detailedAggregation = _PostingAggregator.AggregatePostings(allPostings, postingClassifications, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            await _DataPresenter.WriteErrorsAsync(errorsAndInfos);
            return false;
        }

        detailedAggregation = _AggregatedPostingsNetter.Net(detailedAggregation, inverseClassifications, new List<string>());

        if (!detailedAggregation.Any()) {
            return true;
        }

        var classificationSumList = detailedAggregation.OrderBy(result => result.Key.CombinedClassification).Select(
            result => new TypeItemSum { Type = result.Key.Sign, Item = result.Key.Classification, Sum = result.Value }
        ).OfType<ICollectionViewSourceEntity>().ToList();
        await _DataPresenter.Handlers.ClassificationSumsHandler.CollectionChangedAsync(classificationSumList);
        return true;
    }
}