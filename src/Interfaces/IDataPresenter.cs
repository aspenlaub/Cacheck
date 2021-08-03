namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IDataPresenter {
        ICacheckHandlers Handlers { get; }

        string GetLogText();
    }
}
