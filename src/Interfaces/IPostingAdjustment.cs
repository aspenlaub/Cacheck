using System;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingAdjustment : ICollectionViewSourceEntity {
        DateTime Date { get; }
        string Clue { get; }
        double Amount { get; }
        string Reference { get; set; }
        double AdjustedAmount { get; }
    }
}
