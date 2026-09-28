using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface IPostingsReducer {
    List<ClassifiedPostingDto> RemoveDuplicates(List<ClassifiedPostingDto> dtos, bool export);
}