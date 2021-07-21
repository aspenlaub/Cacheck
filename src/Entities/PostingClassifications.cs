using System.Collections.Generic;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    [XmlRoot("PostingClassifications", Namespace = "http://www.aspenlaub.net")]
    public class PostingClassifications : List<PostingClassification>, ISecretResult<PostingClassifications> {
        public PostingClassifications Clone() {
            var clone = new PostingClassifications();
            clone.AddRange(this);
            return clone;
        }
    }
}
