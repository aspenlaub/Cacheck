using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IrregularDebitClassificationsSecret : ISecret<IrregularDebitClassifications> {
    private IrregularDebitClassifications _PrivateDefaultValue;

    public IrregularDebitClassifications DefaultValue => _PrivateDefaultValue ??= new IrregularDebitClassifications();

    public string Guid => "4067DAFA-42EA-4A58-875F-85117C395FB6";
}