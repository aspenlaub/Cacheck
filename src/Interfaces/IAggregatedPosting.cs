using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IAggregatedPosting {
    public double Sum { get; set; }
    public List<IPosting> Postings { get; }
}