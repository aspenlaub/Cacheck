using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class IndividualPostingEliminationAnalyzer(IPotentialCueExtractor potentialCueExtractor) : IIndividualPostingEliminationAnalyzer {
    public IList<string> AnalyzeClassifiedPostings(IList<IClassifiedPosting> classifiedPostings) {
        var result = new List<string>();
        foreach (IClassifiedPosting classifiedPosting in classifiedPostings.Where(cp => !string.IsNullOrEmpty(cp.Remark))) {
            if (classifiedPosting.Ineliminable || !classifiedPosting.IsIndividual) { continue; }

            var potentialCues = potentialCueExtractor
                .ExtractPotentialCues(classifiedPosting.Remark)
                .Where(cue => !classifiedPostings.Any(cp => !cp.IsIndividual && cp.Remark.Contains(cue)))
                .ToHashSet();
            if (potentialCues.Count == 0) { continue; }

            var classicCues = potentialCues.Where(cue => 1 == classifiedPostings.Count(cp => cp.Remark.Contains(cue))).ToList();
            if (classicCues.Count == 0) { continue; }

            string debitCredit = classifiedPosting.Amount < 0 ? "Debit" : "Credit";
            result.Add($"Found individual posting with hash '{classifiedPosting.PostingHash}' and classification '{classifiedPosting.Classification}' {debitCredit} which could use one of these classic cue/-s:");
            result.AddRange(classicCues.Select(classicCue => $"'{classicCue}'"));
        }

        return result;
    }
}