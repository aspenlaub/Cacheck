using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;

public class TypeMonthDetails : ITypeMonthDetails {
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Item { get; set; }
    public double January { get; set; }
    public double February { get; set; }
    public double March { get; set; }
    public double April { get; set; }
    public double May { get; set; }
    public double June { get; set; }
    public double July { get; set; }
    public double August { get; set; }
    public double September { get; set; }
    public double October { get; set; }
    public double November { get; set; }
    public double December { get; set; }
}