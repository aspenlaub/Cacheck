using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IIndividualPostingEliminationAnalyzer {
    IList<string> AnalyzeClassifiedPostings(IList<IClassifiedPosting> classifiedPostings);
}