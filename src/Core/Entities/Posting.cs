using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Entities {
    public class Posting : IPosting {
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Remark { get; set; }
}
}
