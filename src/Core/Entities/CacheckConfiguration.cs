using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Entities {
    [XmlRoot("CacheckConfiguration")]
    public class CacheckConfiguration : IGuid, ISecretResult<CacheckConfiguration> {
        [XmlAttribute("guid")]
        public string Guid { get; set; }

        [XmlElement("sourcefolder")]
        public string SourceFolder { get; set; }

        public CacheckConfiguration() {
            Guid = System.Guid.NewGuid().ToString();
        }

        public CacheckConfiguration Clone() {
            var clone = (CacheckConfiguration)MemberwiseClone();
            clone.Guid = System.Guid.NewGuid().ToString();
            return clone;
        }
    }
}
