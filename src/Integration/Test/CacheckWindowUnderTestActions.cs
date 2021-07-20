using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    public class CacheckWindowUnderTestActions : WindowUnderTestActionsBase {
        public CacheckWindowUnderTestActions(ITashAccessor tashAccessor) : base(tashAccessor, "Aspenlaub.Net.GitHub.CSharp.Cacheck") {
        }
    }
}
