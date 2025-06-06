﻿using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ICacheckApplicationModel : IApplicationModelBase {
    ICollectionViewSource OverallSums { get; }
    ICollectionViewSource ClassificationSums { get; }
    ICollectionViewSource ClassificationAverages { get; }
    ICollectionViewSource MonthlyDeltas { get; }
    ICollectionViewSource MonthlyDetails { get; }
    ICollectionViewSource ClassifiedPostings { get; }
    ICollectionViewSource SingleMonthDeltas { get; }
    ITextBox Log { get; }
    ISelector SingleClassification { get; }
    ISelector SingleMonth { get; }
    ITextBox LiquidityPlanSum { get; }
    ITextBox ReservationsSum { get; }
    ITextBox MinimumAmount { get; }
    ITextBox FromDay { get; }
    ITextBox ToDay { get; }
}