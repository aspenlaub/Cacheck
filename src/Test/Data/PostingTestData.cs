using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;

public class PostingTestData {
    public readonly IPostingClassification PostingClassificationC1, PostingClassificationC2, PostingClassificationD1, PostingClassificationD2;
    public readonly IPostingClassification PostingClassificationJuly, PostingClassificationAugust, PostingClassificationSeptember;
    public readonly IPostingClassification PostingClassificationD, PostingClassificationC;
    public readonly IPosting PostingC1, PostingD1, PostingC2, PostingD2, PostingC3, PostingD3;
    public const double Amount1 = 10, Amount2 = -20, Amount3 = 30, Amount4 = -40;
    public DateTime Date1 = new(2021, 7, 9), Date2 = new(2021, 7, 10), Date3 = new(2021, 7, 11), Date4 = new(2021, 8, 11);

    public PostingTestData() {
        PostingClassificationC1 = new PostingClassification { Credit = true, Clue = "47", Classification = "4711" };
        PostingClassificationC2 = new PostingClassification { Credit = true, Clue = "24", Classification = "2407" };
        PostingClassificationD1 = new PostingClassification { Credit = false, Clue = "15", Classification = "1510" };
        PostingClassificationD2 = new PostingClassification { Credit = false, Clue = "89", Classification = "1989" };
        PostingClassificationD = new PostingClassification { Credit = false, Clue = "", Classification = "Debit" };
        PostingClassificationC = new PostingClassification { Credit = true, Clue = "", Classification = "Credit" };
        PostingClassificationJuly = new PostingClassification { Month = 7, Year = 2021, Classification = "Δ 2021-07"};
        PostingClassificationAugust = new PostingClassification { Month = 8, Year = 2021, Classification = "Δ 2021-08" };
        PostingClassificationSeptember = new PostingClassification { Month = 9, Year = 2021, Classification = "Δ 2021-09" };

        PostingC1 = new Posting { Date = Date1, Amount = Amount1, Remark = "24789" };
        PostingD1 = new Posting { Date = Date2, Amount = Amount2, Remark = "1589" };
        PostingC2 = new Posting { Date = Date3, Amount = Amount3, Remark = "47" };
        PostingD2 = new Posting { Date = Date4, Amount = Amount4, Remark = "89" };
        PostingC3 = new Posting { Date = Date1, Amount = Amount1, Remark = "72489" };
        PostingD3 = new Posting { Date = Date2, Amount = Amount2, Remark = "8159" };
    }
}