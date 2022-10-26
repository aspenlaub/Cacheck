using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

[XmlRoot("InverseClassifications", Namespace = "http://www.aspenlaub.net")]
public class InverseClassifications : List<InverseClassificationPair>, ISecretResult<InverseClassifications> {
    public InverseClassifications Clone() {
        var clone = new InverseClassifications();
        clone.AddRange(this);
        return clone;
    }
}