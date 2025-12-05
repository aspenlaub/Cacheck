using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public static class PreClassifiedPostingsSettings {
    public static async Task<string> ClassifiedPostingsFileFullNameAsync(IFolderResolver folderResolver, IErrorsAndInfos errorsAndInfos) {
#if DEBUG
        IFolder folder = await folderResolver.ResolveAsync(@"$(MainUserFolder)\Documents\ArborDocs\Cacheck\Qualification\Dump", errorsAndInfos);
#else
        IFolder folder = await folderResolver.ResolveAsync(@"$(MainUserFolder)\Documents\ArborDocs\Cacheck\Production\Dump", errorsAndInfos);
#endif
        if (CacheckApp.IsIntegrationTest) {
            folder = await folderResolver.ResolveAsync(@"$(MainUserFolder)\Documents\ArborDocs\Cacheck\IntegrationTest\Dump", errorsAndInfos);
        }
        folder.CreateIfNecessary();
        return folder.FullName + @"\postings.json";
    }
}
