using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class Posting : IPosting {
        public DateTime Date { get; init; }
        public double Amount { get; init; }
        public string Remark { get; init; }
}
}
