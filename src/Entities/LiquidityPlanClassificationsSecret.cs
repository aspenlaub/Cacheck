using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class LiquidityPlanClassificationsSecret : ISecret<LiquidityPlanClassifications> {
    private LiquidityPlanClassifications _PrivateDefaultValue;

    public LiquidityPlanClassifications DefaultValue => _PrivateDefaultValue ??= new LiquidityPlanClassifications();

    public string Guid => "850A753D-915E-490C-B944-3CA9DB4D3A4F";
}