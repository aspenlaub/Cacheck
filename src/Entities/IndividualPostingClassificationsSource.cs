using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IndividualPostingClassificationsSource : IIndividualPostingClassificationsSource {
    private readonly ISecretRepositoryFactory _SecretRepositoryFactory;
    private ISecretRepository _SecretRepository;

    public IndividualPostingClassificationsSource(ISecretRepositoryFactory secretRepositoryFactory) {
        _SecretRepositoryFactory = secretRepositoryFactory;
        _SecretRepository = secretRepositoryFactory.Create();
    }

    public async Task<IEnumerable<IIndividualPostingClassification>> GetAsync(IErrorsAndInfos errorsAndInfos) {
        return await _SecretRepository.GetAsync(new IndividualPostingClassificationsSecret(), errorsAndInfos);
    }

    public async Task RemoveAsync(IndividualPostingClassification individualPostingClassification) {
        var secret = new IndividualPostingClassificationsSecret();
        var fileName = _SecretRepository.FileName(secret, false, false);
        if (!File.Exists(fileName)) { return; }

        var lines = await File.ReadAllLinesAsync(fileName);
        var newLines = lines.Where(l => !l.Contains(individualPostingClassification.PostingHash)).ToList();
        if (lines.Length == newLines.Count) { return; }

        await File.WriteAllLinesAsync(fileName, newLines);
        _SecretRepository = _SecretRepositoryFactory.Create();
    }

    public async Task AddOrUpdateAsync(IndividualPostingClassification individualPostingClassification, IErrorsAndInfos errorsAndInfos) {
        var secret = new IndividualPostingClassificationsSecret();
        var fileName = _SecretRepository.FileName(secret, false, false);
        if (!File.Exists(fileName)) { return; }

        var lines = (await File.ReadAllLinesAsync(fileName)).Where(l => !string.IsNullOrEmpty(l));
        var newLines = lines.Where(l => !l.Contains(individualPostingClassification.PostingHash)).ToList();
        newLines.Add(newLines[^1]);
        newLines[^2] = "  <IndividualPostingClassification credit=\""
              + (individualPostingClassification.Credit ? "true" : "false")
              + "\" hash=\""
              + individualPostingClassification.PostingHash
              + "\" classification=\""
              + individualPostingClassification.Classification
              + "\" />";

        await File.WriteAllLinesAsync(fileName, newLines);
        _SecretRepository = _SecretRepositoryFactory.Create();
    }
}