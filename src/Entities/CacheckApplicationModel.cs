using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Controls;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Entities;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class CacheckApplicationModel : ApplicationModelBase, ICacheckApplicationModel {
        public ITextBox Summary { get; } = new TextBox();
        public ITextBox Average { get; } = new TextBox();
        public ITextBox MonthlyDelta { get; } = new TextBox();
        public ITextBox Log { get; } = new TextBox();
    }
}
