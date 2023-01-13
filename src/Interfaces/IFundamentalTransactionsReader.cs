using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IFundamentalTransactionsReader {
    IList<Transaction> LoadTransactionsIfAvailable();
}