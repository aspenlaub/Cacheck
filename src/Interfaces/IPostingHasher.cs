namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingHasher {
    string Hash(IPosting posting);
}