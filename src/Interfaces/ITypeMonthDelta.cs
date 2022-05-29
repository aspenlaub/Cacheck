using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ITypeMonthDelta : ICollectionViewSourceEntity {
    string Type { get; set; }
    string Month { get; set; }
    double Delta { get; set; }
}