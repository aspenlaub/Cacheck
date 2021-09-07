using System;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPosting {
        double Amount { get; }
        DateTime Date { get; }
        string Remark { get; }
    }
}