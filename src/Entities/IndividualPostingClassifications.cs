using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

[XmlRoot("IndividualPostingClassifications", Namespace = "http://www.aspenlaub.net")]
public class IndividualPostingClassifications : List<IndividualPostingClassification>, ISecretResult<IndividualPostingClassifications> {
    public IndividualPostingClassifications Clone() {
        var clone = new IndividualPostingClassifications();
        clone.AddRange(this);
        return clone;
    }
}