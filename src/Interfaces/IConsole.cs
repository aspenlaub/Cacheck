using System.Threading.Tasks;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IConsole {
        Task WriteLineAsync(ConsoleOutputType outputType);
        Task WriteLineAsync(ConsoleOutputType outputType, string s);
    }
}
