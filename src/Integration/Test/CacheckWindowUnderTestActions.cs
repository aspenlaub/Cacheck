using Aspenlaub.Net.GitHub.CSharp.TashClient.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.VishnetIntegrationTestTools;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Integration.Test;

public class CacheckWindowUnderTestActions(ITashAccessor tashAccessor)
                : WindowUnderTestActionsBase(tashAccessor, "Aspenlaub.Net.GitHub.CSharp.Cacheck");