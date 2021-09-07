using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data {
    public static class TestDataGenerator {
        public static readonly IList<IPosting> TestPostings = new List<IPosting> {
            new Posting { Date = new DateTime(2020, 1, 1), Amount = 1000, Remark = "Credit Pool 1"},
            new Posting { Date = new DateTime(2020, 1, 4), Amount = -300, Remark = "Debit Pool 1"},
            new Posting { Date = new DateTime(2020, 1, 13), Amount = -200, Remark = "Debit Pool 2"},
            new Posting { Date = new DateTime(2020, 1, 27), Amount = -80, Remark = "Debit Pool 3"},
            new Posting { Date = new DateTime(2020, 2, 1), Amount = 1040, Remark = "Credit Pool 1"},
            new Posting { Date = new DateTime(2020, 2, 12), Amount = -280, Remark = "Debit Pool 1"},
            new Posting { Date = new DateTime(2020, 2, 20), Amount = -240, Remark = "Debit Pool 2"},
            new Posting { Date = new DateTime(2020, 2, 28), Amount = -120, Remark = "Debit Pool 3"},
            new Posting { Date = new DateTime(2020, 3, 1), Amount = 1070, Remark = "Credit Pool 1"},
            new Posting { Date = new DateTime(2020, 3, 1), Amount = -340, Remark = "Debit Pool 1"},
            new Posting { Date = new DateTime(2020, 3, 3), Amount = -270, Remark = "Debit Pool 2"},
            new Posting { Date = new DateTime(2020, 3, 7), Amount = -40, Remark = "Debit Pool 3"},
            new Posting { Date = new DateTime(2020, 4, 1), Amount = 1180, Remark = "Credit Pool 1"},
            new Posting { Date = new DateTime(2020, 4, 10), Amount = -320, Remark = "Debit Pool 1"},
            new Posting { Date = new DateTime(2020, 4, 20), Amount = -240, Remark = "Debit Pool 2"},
            new Posting { Date = new DateTime(2020, 4, 30), Amount = -20, Remark = "Debit Pool 3"},
            new Posting { Date = new DateTime(2020, 5, 1), Amount = 1180, Remark = "Credit Pool 1"},
            new Posting { Date = new DateTime(2020, 5, 4), Amount = -400, Remark = "Debit Pool 1"},
            new Posting { Date = new DateTime(2020, 5, 8), Amount = -200, Remark = "Debit Pool 2"}
        };

        public static void CreateTestFiles() {
            var years = TestPostings.Select(p => p.Date.Year).Distinct().OrderBy(y => y).ToList();
            foreach (var year in years) {
                var months = TestPostings.Where(p => p.Date.Year == year).Select(p => p.Date.Month).Distinct().OrderBy(y => y).ToList();
                foreach (var month in months) {
                    CreateTestFile(year, month);
                }
            }
        }

        private static void CreateTestFile(int year, int month) {
            var postings = TestPostings.Where(p => p.Date.Year == year && p.Date.Month == month).OrderBy(p => p.Date).ToList();
            if (!postings.Any()) { return; }

            var fileName = Folders.IntegrationTestFolder.FullName + @"\Postings_" + year + "_" + month + ".txt";
            var contents = postings.Select(posting => posting.Date.ToShortDateString() + posting.Remark + posting.Date.ToShortDateString() + posting.Amount).ToList();
            File.WriteAllLines(fileName, contents);
        }

        public static void ResetTestFolder() {
            RemoveTestFolder();

            var folder = Folders.IntegrationTestFolder;
            folder.CreateIfNecessary();
            CreateTestFiles();
        }

        public static void RemoveTestFolder() {
            var folder = Folders.IntegrationTestFolder;
            if (!folder.Exists()) { return; }

            var deleter = new FolderDeleter();
            deleter.DeleteFolder(folder);
        }
    }
}
