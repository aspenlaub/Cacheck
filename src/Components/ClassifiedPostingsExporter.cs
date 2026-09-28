using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class ClassifiedPostingsExporter(IPostingsReducer reducer) : IClassifiedPostingsExporter {
    public void ExportClassifiedPostings(string exportFileFullName, IList<IClassifiedPosting> classifiedPostings) {
        List<IPosting> classifiedPostingsDtos = reducer.RemoveDuplicates([.. classifiedPostings.Select(ToDto)], true);
        classifiedPostingsDtos = [.. classifiedPostingsDtos.OrderBy(p => p.Date)];
        string serializedClassifiedPostingsDtos = JsonSerializer.Serialize(classifiedPostingsDtos);
        var fileInfo = new FileInfo(exportFileFullName);
        if (fileInfo.Exists && fileInfo.Length >= serializedClassifiedPostingsDtos.Length) {
            return;
        }

        File.WriteAllText(exportFileFullName, serializedClassifiedPostingsDtos);
    }

    private static ClassifiedPostingDto ToDto(IClassifiedPosting posting) {
        return new ClassifiedPostingDto(posting);
    }
}