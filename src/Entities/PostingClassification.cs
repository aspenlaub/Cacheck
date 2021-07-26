using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class PostingClassification : IPostingClassification {
        [XmlIgnore]
        public bool IgnoreCredit { get; set; }

        [XmlAttribute("credit")]
        public bool Credit { get; set; }

        [XmlAttribute("clue")]
        public string Clue { get; set;  }

        [XmlAttribute("classification")]
        public string Classification { get; set; }

        [XmlIgnore]
        public int Month { get; set; }

        [XmlIgnore]
        public int Year { get; set; }
    }
}
