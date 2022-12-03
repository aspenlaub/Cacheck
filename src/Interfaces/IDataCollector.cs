using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IDataCollector {
    Task CollectAndShowAsync();
}