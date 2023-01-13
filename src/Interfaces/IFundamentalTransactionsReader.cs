using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities.Fundamental;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IFundamentalTransactionsReader {
    Task<IFolder> FundamentalDumpFolderAsync(IErrorsAndInfos errorsAndInfos);
    Task<IList<Transaction>> LoadTransactionsIfAvailableAsync(IErrorsAndInfos errorsAndInfos);
}