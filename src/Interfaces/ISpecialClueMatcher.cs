namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface ISpecialClueMatcher {
        bool DoesPostingMatchSpecialClue(IPosting posting, ISpecialClue classification);
    }
}
