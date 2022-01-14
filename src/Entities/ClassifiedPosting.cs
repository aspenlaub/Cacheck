using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class ClassifiedPosting : IClassifiedPosting {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Classification { get; set; }
        public string Clue { get; set; }
        public string Remark { get; set; }
        public string FormattedDate => $"{Date.Day:D2}.{Date.Month:D2}.{Date.Year}";
    }
}