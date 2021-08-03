using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions {
    public static class DataPresenterExtensions {
        public static async Task WriteErrorsAsync(this IDataPresenter dataPresenter, IErrorsAndInfos errorsAndInfos) {
            var errors = errorsAndInfos.Errors.ToList();
            foreach (var error in errors) {
                await dataPresenter.WriteLineAsync(error);
            }
        }

        public static async Task WriteLineAsync(this IDataPresenter dataPresenter, string s) {
            var logText = dataPresenter.GetLogText();
            var textHandler = dataPresenter.Handlers.LogTextHandler;
            s = logText + (string.IsNullOrEmpty(logText) ? "" : "\r\n") + s;
            await textHandler.TextChangedAsync(s);
        }
    }
}
