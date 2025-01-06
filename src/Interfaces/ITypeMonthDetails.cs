using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

public interface ITypeMonthDetails : ICollectionViewSourceEntity {
    string Type { get; set; }
    string Item { get; set; }
    double January { get; set; }
    double February { get; set; }
    double March { get; set; }
    double April { get; set; }
    double May { get; set; }
    double June { get; set; }
    double July { get; set; }
    double August { get; set; }
    double September { get; set; }
    double October { get; set; }
    double November { get; set; }
    double December { get; set; }
}