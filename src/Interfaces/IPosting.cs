using System;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPosting : IGuid {
    double Amount { get; }
    DateTime Date { get; }
    string Remark { get; }
}