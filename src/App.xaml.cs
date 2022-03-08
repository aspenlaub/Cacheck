using System.Linq;
using System.Windows;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck {
    /// <summary>
    /// Interaction logic for CacheckApp.xaml
    /// </summary>
    public partial class CacheckApp {
        public static bool IsIntegrationTest { get; private set; }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            IsIntegrationTest = e.Args.Any(a => a == "/UnitTest");
        }
    }
}
