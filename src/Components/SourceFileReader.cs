using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SourceFileReader : ISourceFileReader {
    public IList<IPosting> ReadPostings(string fileName, IErrorsAndInfos errorsAndInfos) {
        var postings = new List<IPosting>();
        if (!File.Exists(fileName)) {
            errorsAndInfos.Errors.Add($"File \"{fileName}\" does not exist");
            return postings;
        }

        var lines = File.ReadAllLines(fileName).ToList();
        for(var i = 0; i < lines.Count; i ++) {
            if (!TryReadPostings(lines[i], out var posting)) { continue; }

            var unreadableLines = new List<string>();
            int j;
            for (j = i + 1; j < lines.Count && j < i + 2 && !TryReadPostings(lines[j], out _); j ++) {
                unreadableLines.Add(lines[j].Trim());
            }

            var numberOfDigits = string.Join("", unreadableLines).ToCharArray().Count(char.IsDigit);
            if (numberOfDigits < 10) {
                for (; j < lines.Count && j < i + 4 && !TryReadPostings(lines[j], out _); j++) {
                    unreadableLines.Add(lines[j].Trim());
                }
            }

            if (unreadableLines.Any()) {
                posting = new Posting {
                    Amount = posting.Amount,
                    Date = posting.Date,
                    Remark = posting.Remark + '/' + string.Join('/', unreadableLines)
                };
            }
            postings.Add(posting);
        }
        return postings;
    }

    private static bool TryReadPostings(string line, out IPosting posting) {
        posting = null;
        line = line.Trim();
        if (line.Length < 20) {
            return false;
        }

        var dates = new List<DateTime>();
        var datePositions = new List<int>();
        for (var i = 0; i < line.Length - 10; i++) {
            var dateString = line.Substring(i, 10);
            if (!DateTime.TryParse(dateString, out var date)) {
                continue;
            }

            if (dateString != date.ToShortDateString()) {
                continue;
            }

            dates.Add(date);
            datePositions.Add(i);
        }

        if (dates.Count < 2) {
            return false;
        }

        if (!double.TryParse(line[(datePositions[dates.Count - 1] + 10)..], out var amount)) {
            return false;
        }

        posting = new Posting {
            Date = dates[0],
            Amount = amount,
            Remark = line.Substring(datePositions[0] + 10, datePositions[dates.Count - 1] - datePositions[0] - 10)
        };
        return true;
    }
}