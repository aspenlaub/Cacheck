using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPotentialCueExtractor {
    HashSet<string> ExtractPotentialCues(string postingRemark);
}