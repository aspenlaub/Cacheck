using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class AggregatedPosting : IAggregatedPosting {
    public double Sum { get; set; }
    public List<IPosting> Postings { get; } = [];
}