using System.Linq;
using System.Windows;
using Aspenlaub.Net.GitHub.CSharp.VishizhukelNet.Helpers;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App {
    /// <summary>
    /// Interaction logic for CacheckApp.xaml
    /// </summary>
    public partial class CacheckApp {
        public static bool IsIntegrationTest { get; private set; }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            IsIntegrationTest = e.Args.Any(a => a == "/UnitTest");
            ExceptionHandlerUpSetter.SetUp(this);
        }
    }
}
