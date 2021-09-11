using System;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingAdjustment {
        DateTime Date { get; }
        string Clue { get; }
        double Amount { get; }
        double AdjustedAmount { get; }
    }
}
