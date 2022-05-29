namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingClassificationFormatter {
    IFormattedClassification Format(IPostingClassification classification);
}