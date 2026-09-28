using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingsReducer : IPostingsReducer {
    public List<IPosting> RemoveDuplicates(List<IPosting> dtos, bool export) {
        if (dtos.Count == 0) {
            return dtos;
        }
        var dtoStrings = dtos.Select(x => x.ToString()).ToList();
        var uniqueDtoStrings = dtoStrings.Distinct().ToList();
        return (export && uniqueDtoStrings.Count != dtoStrings.Count)
            ? throw new NotSupportedException("There should not be any duplicates")
            : (uniqueDtoStrings.Count * 10 < dtoStrings.Count * 9)
                ?  throw new NotSupportedException("Too many duplicates")
                : uniqueDtoStrings.Count == 1
                    ? throw new NotSupportedException($"Override ToString for {dtoStrings[0]}")
                    : [.. uniqueDtoStrings.Select(x => dtos[dtoStrings.IndexOf(x)])];
    }
}