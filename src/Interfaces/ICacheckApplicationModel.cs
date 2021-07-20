using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ICacheckApplicationModel : IApplicationModel {
        ITextBox ConsoleOutput { get; }
    }
}
