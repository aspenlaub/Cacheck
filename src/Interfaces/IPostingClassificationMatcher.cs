namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingClassificationMatcher {
        bool DoesPostingMatchClassification(IPosting posting, IPostingClassification classification);
    }
}
