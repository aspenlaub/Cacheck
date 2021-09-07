using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class SpecialClue : ISpecialClue {
        [XmlAttribute("clue")]
        public string Clue { get; set; }
    }
}