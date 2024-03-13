using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IndividualPostingClassification : IIndividualPostingClassification  {
    [XmlAttribute("credit")]
    public bool Credit { get; init; }

    [XmlAttribute("hash")]
    public string PostingHash { get; init; }

    [XmlAttribute("classification")]
    public string Classification { get; init; }

    [XmlAttribute("ineliminable")]
    public bool Ineliminable { get; init; }
}