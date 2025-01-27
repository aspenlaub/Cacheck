using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PotentialCueExtractor : IPotentialCueExtractor {
    private const int _minimumExtractedCueLength = 12;

    public HashSet<string> ExtractPotentialCues(string postingRemark) {
        var parts = new HashSet<string>();
        int maxStartPos = postingRemark.Length - _minimumExtractedCueLength;
        for (int startPos = 0; startPos <= maxStartPos;) {
            startPos = FindLetter(postingRemark, startPos, maxStartPos);
            if (startPos > maxStartPos) { break; }

            int endPos = FindEndPosition(postingRemark, startPos);

            string part = postingRemark.Substring(startPos, endPos - startPos + 1);
            if (part.Length >= _minimumExtractedCueLength) {
                parts.UnionWith(FindSubParts(part));
            }

            startPos = endPos + 1;
        }

        return parts;
    }

    private int FindLetter(string postingRemark, int startPos, int maxStartPos) {
        for (; startPos <= maxStartPos && !IsCueCharacter(postingRemark[startPos]); startPos++) {
        }

        return startPos;
    }

    private int FindEndPosition(string postingRemark, int startPos) {
        int endPos;
        for (endPos = startPos;
             endPos < postingRemark.Length
             && (IsCueCharacter(postingRemark[endPos]) || postingRemark[endPos] == ' ');
             endPos++) {
        }

        for (endPos--; endPos > startPos && postingRemark[endPos] == ' '; endPos--) {
        }

        return endPos;
    }

    private HashSet<string> FindSubParts(string part) {
        if (part.Length < _minimumExtractedCueLength) { return []; }

        var parts = new HashSet<string> { part };
        for (int startPos = 2; startPos < part.Length; startPos++) {
            if (part[startPos - 1] != ' ' || part[startPos] == ' ') { continue; }

            parts.UnionWith(FindSubParts(part.Substring(0, startPos - 1).TrimEnd()));
            parts.UnionWith(FindSubParts(part.Substring(startPos)));
        }
        return parts;
    }

    private static bool IsCueCharacter(char c) {
        return char.IsLetter(c) || c == ',' || c == '-';
    }
}