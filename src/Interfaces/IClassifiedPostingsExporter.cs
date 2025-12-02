using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IClassifiedPostingsExporter {
    void ExportClassifiedPostings(string exportFileFullName, IList<IClassifiedPosting> classifiedPostings);
}
