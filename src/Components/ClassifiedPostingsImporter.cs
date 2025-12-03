using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class ClassifiedPostingsImporter : IClassifiedPostingsImporter {
    public async Task<IList<IPosting>> ImportClassifiedPostingsAsync(string importFileFullName, IErrorsAndInfos errorsAndInfos) {
        if (!File.Exists(importFileFullName)) {
            errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileNotFound, importFileFullName));
            return [];
        }

        List<IPosting> classifiedPostingDtos;
        try {
            string json = await File.ReadAllTextAsync(importFileFullName);
            classifiedPostingDtos = [.. JsonSerializer.Deserialize<List<ClassifiedPostingDto>>(json)];
        } catch (Exception e) {
            errorsAndInfos.Errors.Add(string.Format(Properties.Resources.FileCouldNotBeDeserialized, importFileFullName, e.Message));
            return [];
        }
        return classifiedPostingDtos;
    }
}
