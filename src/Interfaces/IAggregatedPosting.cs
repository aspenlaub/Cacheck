using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IAggregatedPosting {
    double Sum { get; set; }
    List<IPosting> Postings { get; }
}