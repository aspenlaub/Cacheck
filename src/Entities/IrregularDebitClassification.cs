using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IrregularDebitClassification {
    [XmlAttribute("classification")]
    public string Classification { get; init; }

    [XmlAttribute("percentage")]
    public int Percentage { get; init; }
}