using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SourceFileReader : ISourceFileReader {
    public IList<IPosting> ReadPostings(string fileName, IErrorsAndInfos errorsAndInfos) {
        ErrorsAndInfos readingErrorsAndInfos = new ErrorsAndInfos();
        IList<IPosting> postings = ReadPostings(fileName, readingErrorsAndInfos,
            (x, _) => ReadBefore2025Posting(x), _ => null);
        if (postings.Count != 0) {
            foreach (string s in readingErrorsAndInfos.Infos) {
                errorsAndInfos.Infos.Add(s);
            }
            foreach (string s in readingErrorsAndInfos.Errors) {
                errorsAndInfos.Errors.Add(s);
            }
            return postings;
        }

        postings = ReadPostings(fileName, errorsAndInfos, ReadPosting, ReadFromDate);
        return postings;
    }

    protected IList<IPosting> ReadPostings(string fileName, IErrorsAndInfos errorsAndInfos,
            Func<string, DateTime?, IPosting> readPosting, Func<string, DateTime?> readFromDate) {
        List<IPosting> postings = [];
        if (!File.Exists(fileName)) {
            errorsAndInfos.Errors.Add($"File \"{fileName}\" does not exist");
            return postings;
        }

        List<string> lines = File.ReadAllLines(fileName).ToList();

        DateTime? fromDate = null;
        foreach (var line in lines) {
            fromDate = readFromDate(line);
            if (fromDate != null) {
                break;
            }
        }

        for(int i = 0; i < lines.Count; i ++) {
            IPosting posting = readPosting(lines[i], fromDate);
            if (posting == null) { continue; }

            List<string> unreadableLines = [];
            int j;
            for (j = i + 1; j < lines.Count && j < i + 2 && readPosting(lines[j], fromDate) == null; j ++) {
                unreadableLines.Add(lines[j].Trim());
            }

            int numberOfDigits = string.Join("", unreadableLines).ToCharArray().Count(char.IsDigit);
            if (numberOfDigits < 10) {
                for (; j < lines.Count && j < i + 4 && readPosting(lines[j], fromDate) == null; j++) {
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

    private static IPosting ReadBefore2025Posting(string line) {
        line = line.Trim();
        if (line.Length < 20) {
            return null;
        }

        List<DateTime> dates = [];
        List<int> datePositions = [];
        for (int i = 0; i < line.Length - 10; i++) {
            string dateString = line.Substring(i, 10);
            if (!DateTime.TryParse(dateString, out DateTime date)) {
                continue;
            }

            if (dateString != date.ToShortDateString()) {
                continue;
            }

            dates.Add(date);
            datePositions.Add(i);
        }

        if (dates.Count < 2) {
            return null;
        }

        double amount;
        return double.TryParse(line[(datePositions[dates.Count - 1] + 10)..], out amount)
            ? new Posting {
                Date = dates[0],
                Amount = amount,
                Remark = line.Substring(datePositions[0] + 10, datePositions[dates.Count - 1] - datePositions[0] - 10)
            }
            : null;
    }

    private DateTime? ReadFromDate(string line) {
        var pos = line.IndexOf("vom ", StringComparison.InvariantCultureIgnoreCase);
        if (pos < 0) { return null; }

        line = line.Substring(pos + 4).Trim() + " ";
        line = line.Substring(0, line.IndexOf(' '));
        if (!DateTime.TryParse(line, out DateTime fromDate)) {
            return null;
        }

        return fromDate;
    }

    private static IPosting ReadPosting(string line, DateTime? fromDate) {
        if (fromDate == null) {
            return null;
        }

        line = line.Trim();
        if (line.Length < 20) {
            return null;
        }

        ReadDayAndMonth(ref line, out int day, out int _);
        if (day <= 0) { return null; }

        ReadDayAndMonth(ref line, out day, out int month);
        if (day <= 0) { return null; }

        DateTime date;
        if (month == fromDate.Value.Month || month == fromDate.Value.Month + 1) {
            date = new DateTime(fromDate.Value.Year, month, day);
        } else if (month == fromDate.Value.Month - 1) {
            throw new NotImplementedException();
        } else {
            throw new NotImplementedException();
        }

        if (line.Length < 5) {
            throw new NotImplementedException();
        }

        int sign;
        switch (line[^1]) {
            case 'H':
                sign = 1;
                break;
            case 'S':
                sign = -1;
                break;
            default:
                throw new NotImplementedException();
        }

        line = line.Substring(0, line.Length - 1).Trim();

        if (line.Length < 5) {
            throw new NotImplementedException();
        }

        var pos = line.LastIndexOf(' ');
        if (pos < 0) {
            throw new NotImplementedException();
        }

        if (!double.TryParse(line.Substring(pos + 1), out double amount)) {
            throw new NotImplementedException();
        }

        var remark = line.Substring(0, pos).Trim();

        return new Posting {
            Date = date,
            Amount = amount * sign,
            Remark = remark
        };
    }

    private static void ReadDayAndMonth(ref string line, out int day, out int month) {
        day = -1;
        month = -1;

        if (line.Length < 6) { return; }

        if (!Char.IsDigit(line[0])) { return; }
        if (!Char.IsDigit(line[1])) { return; }
        if (line[2] != '.') { return; }
        if (!Char.IsDigit(line[3])) { return; }
        if (!Char.IsDigit(line[4])) { return; }
        if (line[5] != '.') { return; }

        day = int.Parse(line.Substring(0, 2));
        month = int.Parse(line.Substring(3, 2));
        line = line.Substring(6).Trim();
    }
}