using System;
using System.Collections.Generic;
using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class SourceFileReader : ISourceFileReader {
        public IList<IPosting> ReadPostings(string fileName, IErrorsAndInfos errorsAndInfos) {
            var postings = new List<IPosting>();
            if (!File.Exists(fileName)) {
                errorsAndInfos.Errors.Add($"File \"{fileName}\" does not exist");
                return postings;
            }

            var lines = File.ReadAllLines(fileName);
            foreach (var line in lines) {
                if (!TryReadPostings(line, out var posting)) { continue; }

                postings.Add(posting);
            }
            return postings;
        }

        private bool TryReadPostings(string line, out IPosting posting) {
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

            posting = new Posting {
                Date = dates[0],
                Amount = double.Parse(line.Substring(datePositions[dates.Count - 1] + 10)),
                Remark = line.Substring(datePositions[0] + 10, datePositions[dates.Count - 1] - datePositions[0] - 10)
            };
            return true;
        }
    }
}