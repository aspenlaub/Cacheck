using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class IrregularDebitClassification : IIrregularDebitClassification {
    [XmlAttribute("classification")]
    public string Classification { get; init; }

    [XmlAttribute("percentage")]
    public int Percentage { get; init; }
}