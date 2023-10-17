using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

[XmlRoot("LiquidityPlanClassifications", Namespace = "http://www.aspenlaub.net")]
public class LiquidityPlanClassifications : List<LiquidityPlanClassification>, ISecretResult<LiquidityPlanClassifications> {
    public LiquidityPlanClassifications Clone() {
        var clone = new LiquidityPlanClassifications();
        clone.AddRange(this);
        return clone;
    }
}