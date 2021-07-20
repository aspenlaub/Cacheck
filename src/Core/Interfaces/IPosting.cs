using System;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Interfaces {
    public interface IPosting {
        double Amount { get; set; }
        DateTime Date { get; set; }
        string Remark { get; set; }
    }
}