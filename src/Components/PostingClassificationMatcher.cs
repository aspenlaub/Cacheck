﻿using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingClassificationMatcher : IPostingClassificationMatcher {
        public const int DayOfMonthBalance = 10;

        public bool DoesPostingMatchClassification(IPosting posting, IPostingClassification classification) {
            return DoesPostingMatchDebitCredit(posting, classification) && DoesPostingMatchClue(posting, classification) && DoesPostingMatchYearAndMonth(posting, classification);
        }

        private static bool DoesPostingMatchDebitCredit(IPosting posting, IPostingClassification classification) {
            return classification.IgnoreCredit || classification.Credit == posting.Amount > 0 || !classification.Credit == posting.Amount < 0;
        }

        private static bool DoesPostingMatchClue(IPosting posting, IPostingClassification classification) {
            return string.IsNullOrEmpty(classification.Clue) || posting.Remark.Contains(classification.Clue, StringComparison.OrdinalIgnoreCase);
        }

        private static bool DoesPostingMatchYearAndMonth(IPosting posting, IPostingClassification classification) {
            if (classification.Month == 0 && classification.Year == 0) { return true; }

            var year = posting.Date.Year;
            var month = posting.Date.Day <= DayOfMonthBalance ? posting.Date.Month : posting.Date.Month + 1;
            return month == 13 ? classification.Month == 1 && classification.Year == year + 1 : classification.Month == month && classification.Year == year;
        }
    }
}
