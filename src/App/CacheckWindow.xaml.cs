using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.App {
    /// <summary>
    /// Interaction logic for CacheckWindow.xaml
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public partial class CacheckWindow {
        private static IContainer Container { get; set; }

        public CacheckWindow() {
            InitializeComponent();

            var builder = new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(this).Result;
            Container = builder.Build();
        }
    }
}
