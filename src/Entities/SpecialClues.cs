using System.Collections.Generic;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    [XmlRoot("PostingClassifications", Namespace = "http://www.aspenlaub.net")]
    public class SpecialClues : List<SpecialClue>, ISecretResult<SpecialClues> {
        public SpecialClues Clone() {
            var clone = new SpecialClues();
            clone.AddRange(this);
            return clone;
        }
    }
}