using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class FundamentalTransactionsReader : IFundamentalTransactionsReader {
    public IList<Transaction> LoadTransactionsIfAvailable() {
        return new List<Transaction>();
    }
}