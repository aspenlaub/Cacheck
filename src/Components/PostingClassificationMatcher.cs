﻿using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingClassificationMatcher(IPostingHasher postingHasher) : IPostingClassificationMatcher {

    public bool DoesPostingMatchClassification(IPosting posting, IPostingClassification classification) {
        return classification.IsUnassigned
            ? DoesPostingMatchUnassignedClassification(posting,classification)
                : classification.IsIndividual
                ? DoesPostingMatchIndividualClassification(posting, classification)
                : DoesPostingMatchDebitCredit(posting, classification)
                  && DoesPostingMatchClue(posting, classification)
                  && DoesPostingMatchYearAndMonth(posting, classification);
    }

    private static bool DoesPostingMatchDebitCredit(IPosting posting, IPostingClassification classification) {
        return classification.IsMonthClassification || classification.Credit == posting.Amount > 0 || !classification.Credit == posting.Amount < 0;
    }

    private static bool DoesPostingMatchClue(IPosting posting, IPostingClassification classification) {
        return string.IsNullOrEmpty(classification.Clue) || posting.Remark.Contains(classification.Clue, StringComparison.OrdinalIgnoreCase);
    }

    private static bool DoesPostingMatchYearAndMonth(IPosting posting, IPostingClassification classification) {
        return classification.Month == 0 && classification.Year == 0
            || classification.Month == posting.Date.Month && classification.Year == posting.Date.Year;
    }

    private bool DoesPostingMatchIndividualClassification(IPosting posting, IPostingClassification classification) {
        return postingHasher.Hash(posting) == classification.Clue;
    }

    private bool DoesPostingMatchUnassignedClassification(IPosting posting, IPostingClassification classification) {
        return classification.Credit == posting.Amount >= 0;
    }
}