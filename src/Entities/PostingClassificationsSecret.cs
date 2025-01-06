using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class PostingClassificationsSecret : ISecret<PostingClassifications> {
    private PostingClassifications _PrivateDefaultValue;

    public PostingClassifications DefaultValue => _PrivateDefaultValue ??= [new() { Credit = true, Clue = "a clue", Classification = "a classification" }];

    public string Guid => "EB563C4A-E590-293B-1340-EDB2545E8A95";
}