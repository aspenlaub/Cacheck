using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class InverseClassificationPair : IInverseClassificationPair {
    [XmlAttribute("classification")]
    public string Classification { get; init; }

    [XmlAttribute("inverse")]
    public string InverseClassification { get; init; }
}