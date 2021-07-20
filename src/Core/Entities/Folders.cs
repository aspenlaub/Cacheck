using System.IO;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Core.Entities {
    public class Folders {
        public static readonly IFolder IntegrationTestFolder = new Folder(Path.GetTempPath()).SubFolder("CacheckTestData");
    }
}
