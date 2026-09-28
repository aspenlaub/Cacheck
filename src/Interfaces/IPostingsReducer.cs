using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingsReducer {
    List<IPosting> RemoveDuplicates(List<IPosting> dtos, bool export);
}