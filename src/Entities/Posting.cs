using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class Posting : IPosting {
        public string Guid { get; init; } = System.Guid.NewGuid().ToString();
        public DateTime Date { get; init; }
        public double Amount { get; init; }
        public string Remark { get; init; }
    }
}
