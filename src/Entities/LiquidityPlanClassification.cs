using System.Xml.Serialization;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class LiquidityPlanClassification {
    [XmlAttribute("classification")]
    public string Classification { get; init; }

    [XmlAttribute("liquidityclassification")]
    public string LiquidityClassification { get; init; }

    [XmlAttribute("percentage")]
    public int Percentage { get; init; }

    [XmlAttribute("target")]
    public double Target { get; init; }
}