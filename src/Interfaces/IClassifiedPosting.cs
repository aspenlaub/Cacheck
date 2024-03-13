using System;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IClassifiedPosting : ICollectionViewSourceEntity {
    DateTime Date { get; set; }
    double Amount { get; set; }
    string Classification { get; set; }
    string Clue { get; set; }
    string Remark { get; set; }
    bool IsIndividual { get; set; }
    string PostingHash { get; set; }
    bool Ineliminable { get; set; }
}