using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class SecretRepositoryFactory : ISecretRepositoryFactory {
    public ISecretRepository Create() {
        var container = new ContainerBuilder().UsePegh("Cacheck", new DummyCsArgumentPrompter()).Build();
        return container.Resolve<ISecretRepository>();
    }
}