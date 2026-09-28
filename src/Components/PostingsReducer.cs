using System;
using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class PostingsReducer : IPostingsReducer {
    public List<ClassifiedPostingDto> RemoveDuplicates(List<ClassifiedPostingDto> dtos, bool export) {
        if (dtos.Count == 0) {
            return dtos;
        }
        var dtoStrings = dtos.Select(x => x.ToString()).ToList();
        var duplicateDtoStrings = dtoStrings.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        var uniqueDtoStrings = dtoStrings.Distinct().ToList();
        return (export && duplicateDtoStrings.Count != 0)
            ? throw new NotSupportedException("There should not be any duplicates")
            : (duplicateDtoStrings.Count * 10 > dtoStrings.Count)
                ?  throw new NotSupportedException("Too many duplicates")
                : uniqueDtoStrings.Count == 1
                    ? throw new NotSupportedException($"Override ToString for {dtoStrings[0]}")
                    : [.. uniqueDtoStrings.Select(x => dtos[dtoStrings.IndexOf(x)])];
    }
}