using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class InverseClassificationsSecret : ISecret<InverseClassifications> {
    public InverseClassifications DefaultValue
        => field ??= [new() { Classification = "Fees", InverseClassification = "FeesRepay" }];

    public string Guid => "CEE85938-E84F-496B-8218-19C5998EFE4D";
}