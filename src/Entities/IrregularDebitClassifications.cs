using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

[XmlRoot("IrregularDebitClassifications", Namespace = "http://www.aspenlaub.net")]
public class IrregularDebitClassifications : List<IrregularDebitClassification>, ISecretResult<IrregularDebitClassifications> {
    public IrregularDebitClassifications Clone() {
        var clone = new IrregularDebitClassifications();
        clone.AddRange(this);
        return clone;
    }
}