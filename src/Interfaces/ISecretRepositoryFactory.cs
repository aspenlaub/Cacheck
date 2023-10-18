using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ISecretRepositoryFactory {
        ISecretRepository Create();
    }
}
