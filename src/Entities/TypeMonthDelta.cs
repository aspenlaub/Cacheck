using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class TypeMonthDelta : ITypeMonthDelta {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Type { get; set; }
        public string Month { get; set; }
        public double Delta { get; set; }
    }
}
