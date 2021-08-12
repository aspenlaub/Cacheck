using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data {
    public class PostingTestData {
        public IPostingClassification PostingClassificationC1, PostingClassificationC2, PostingClassificationD1, PostingClassificationD2;
        public IPostingClassification PostingClassificationJuly, PostingClassificationAugust, PostingClassificationSeptember;
        public IPostingClassification PostingClassificationD, PostingClassificationC;
        public IPosting PostingC1, PostingD1, PostingC2, PostingD2, PostingC3, PostingD3;
        public double Amount1 = 10, Amount2 = -20, Amount3 = 30, Amount4 = -40;
        public DateTime Date1 = new(2021, 7, 9), Date2 = new(2021, 7, 10), Date3 = new(2021, 7, 11), Date4 = new(2021, 8, 11);

        public PostingTestData() {
            PostingClassificationC1 = new PostingClassification { Credit = true, Clue = "47", Classification = "4711" };
            PostingClassificationC2 = new PostingClassification { Credit = true, Clue = "24", Classification = "2407" };
            PostingClassificationD1 = new PostingClassification { Credit = false, Clue = "15", Classification = "1510" };
            PostingClassificationD2 = new PostingClassification { Credit = false, Clue = "89", Classification = "1989" };
            PostingClassificationD = new PostingClassification { Credit = false, Clue = "", Classification = "Debit" };
            PostingClassificationC = new PostingClassification { Credit = true, Clue = "", Classification = "Credit" };
            PostingClassificationJuly = new PostingClassification { IgnoreCredit = true, Month = 7, Year = 2021 };
            PostingClassificationAugust = new PostingClassification { IgnoreCredit = true, Month = 8, Year = 2021, ExcludeFromFairCalculation = true };
            PostingClassificationSeptember = new PostingClassification { IgnoreCredit = true, Month = 9, Year = 2021 };

            PostingC1 = new Posting { Date = Date1, Amount = Amount1, Remark = "24789" };
            PostingD1 = new Posting { Date = Date2, Amount = Amount2, Remark = "1589" };
            PostingC2 = new Posting { Date = Date3, Amount = Amount3, Remark = "47" };
            PostingD2 = new Posting { Date = Date4, Amount = Amount4, Remark = "89" };
            PostingC3 = new Posting { Date = Date1, Amount = Amount1, Remark = "72489" };
            PostingD3 = new Posting { Date = Date2, Amount = Amount2, Remark = "8159" };
        }
    }
}
