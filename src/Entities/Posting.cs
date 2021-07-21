using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class Posting : IPosting {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Remark { get; set; }
}
}
