using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IndividualPostingClassificationsSecret : ISecret<IndividualPostingClassifications> {
    private IndividualPostingClassifications _PrivateDefaultValue;

    public IndividualPostingClassifications DefaultValue
        => _PrivateDefaultValue ??= [];

    public string Guid => "1C3A5F6E-485C-4B08-801A-EFF63F05CCDC";
}