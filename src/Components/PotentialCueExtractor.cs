using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PotentialCueExtractor : IPotentialCueExtractor {
    private const int MinimumExtractedCueLength = 12;

    public HashSet<string> ExtractPotentialCues(string postingRemark) {
        var parts = new HashSet<string>();
        var maxStartPos = postingRemark.Length - MinimumExtractedCueLength;
        for (var startPos = 0; startPos <= maxStartPos;) {
            startPos = FindLetter(postingRemark, startPos, maxStartPos);
            if (startPos > maxStartPos) { break; }

            var endPos = FindEndPosition(postingRemark, startPos);

            var part = postingRemark.Substring(startPos, endPos - startPos + 1);
            if (part.Length >= MinimumExtractedCueLength) {
                parts.UnionWith(FindSubParts(part));
            }

            startPos = endPos + 1;
        }

        return parts;
    }

    private int FindLetter(string postingRemark, int startPos, int maxStartPos) {
        for (; startPos <= maxStartPos && !char.IsLetter(postingRemark[startPos]); startPos++) {
        }

        return startPos;
    }

    private int FindEndPosition(string postingRemark, int startPos) {
        int endPos;
        for (endPos = startPos;
             endPos < postingRemark.Length
             && (char.IsLetter(postingRemark[endPos]) || postingRemark[endPos] == ' ');
             endPos++) {
        }

        for (endPos--; endPos > startPos && postingRemark[endPos] == ' '; endPos--) {
        }

        return endPos;
    }

    private HashSet<string> FindSubParts(string part) {
        if (part.Length < MinimumExtractedCueLength) { return new HashSet<string>(); }

        var parts = new HashSet<string> { part };
        for (var startPos = 2; startPos < part.Length; startPos++) {
            if (part[startPos - 1] != ' ' || part[startPos] == ' ') { continue; }

            parts.UnionWith(FindSubParts(part.Substring(0, startPos - 1).TrimEnd()));
            parts.UnionWith(FindSubParts(part.Substring(startPos)));
        }
        return parts;
    }
}