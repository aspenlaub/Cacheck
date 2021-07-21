using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IDataPresenter {
        Task WriteLineAsync(DataPresentationOutputType outputType);
        Task WriteLineAsync(DataPresentationOutputType outputType, string s);

        Task WriteErrorsAsync(IErrorsAndInfos errorsAndInfos);
    }
}
