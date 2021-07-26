using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ICacheckApplicationModel : IApplicationModel {
        ITextBox Summary { get; }
        ITextBox Average { get; }
        ITextBox MonthlyDelta { get; }
        ITextBox Log { get; }
    }
}
