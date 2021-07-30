using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    public class FormattedClassification : IFormattedClassification {
        public string Sign { get; set; }
        public string Classification { get; set; }

        public string CombinedClassification { get; set; }
    }
}
