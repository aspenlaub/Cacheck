using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IndividualPostingClassificationsSecret : ISecret<IndividualPostingClassifications> {
    public IndividualPostingClassifications DefaultValue => field ??= [];

    public string Guid => "1C3A5F6E-485C-4B08-801A-EFF63F05CCDC";
}