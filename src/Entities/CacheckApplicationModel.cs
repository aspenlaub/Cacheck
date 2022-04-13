﻿using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Controls;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class CacheckApplicationModel : ApplicationModelBase<FakeWebBrowserOrView>, ICacheckApplicationModel {
        public ICollectionViewSource OverallSums { get; } = new CollectionViewSource { EntityType = typeof(TypeItemSum) };
        public ICollectionViewSource ClassificationSums { get; } = new CollectionViewSource { EntityType = typeof(TypeItemSum) };
        public ICollectionViewSource ClassificationAverages { get; } = new CollectionViewSource { EntityType = typeof(TypeItemSum) };
        public ICollectionViewSource MonthlyDeltas { get; } = new CollectionViewSource { EntityType = typeof(TypeMonthDelta) };
        public ICollectionViewSource ClassifiedPostings { get; } = new CollectionViewSource { EntityType = typeof(ClassifiedPosting) };
        public ITextBox Log { get; } = new TextBox();
    }
}
