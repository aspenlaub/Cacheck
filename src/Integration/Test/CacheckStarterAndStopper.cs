using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test {
    public class CacheckStarterAndStopper : StarterAndStopperBase {
        protected override string ProcessName => "Aspenlaub.Net.GitHub.CSharp.Cacheck";
        protected override List<string> AdditionalProcessNamesToStop => new();

        protected override string ExecutableFile() {
            return typeof(CacheckWindowUnderTest).Assembly.Location
                .Replace(@"\Integration\Test\", @"\")
                .Replace("Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test.dll", ProcessName + ".exe");
        }
    }
}