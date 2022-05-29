using System.Threading.Tasks;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IDataCollector {
    Task CollectAndShowAsync(IContainer container, bool isIntegrationTest);
}