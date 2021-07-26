using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    public class FakeDataPresenter : IDataPresenter {
        public IDictionary<DataPresentationOutputType, IList<string>> Output = new Dictionary<DataPresentationOutputType, IList<string>>();

        public async Task WriteLineAsync(DataPresentationOutputType outputType) {
            await WriteLineAsync(outputType, "");
        }

        public async Task WriteLineAsync(DataPresentationOutputType outputType, string s) {
            if (!Output.ContainsKey(outputType)) {
                Output[outputType] = new List<string>();
            }
            Output[outputType].Add(s);
            await Task.CompletedTask;
        }

        public async Task WriteErrorsAsync(IErrorsAndInfos errorsAndInfos) {
            await Task.CompletedTask;
            throw new NotImplementedException("Errors are not expected");
        }
    }
}
