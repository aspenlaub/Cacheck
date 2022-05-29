using System;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
// ReSharper disable UnusedMember.Global

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IClassifiedPosting : ICollectionViewSourceEntity {
    DateTime Date { get; set; }
    double Amount { get; set; }
    string Classification { get; set; }
    string Clue { get; set; }
    string Remark { get; set; }
}