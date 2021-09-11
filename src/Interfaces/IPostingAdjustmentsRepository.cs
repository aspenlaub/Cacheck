using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces {
    public interface IPostingAdjustmentsRepository {
        IList<IPostingAdjustment> LoadAdjustments(IFolder sourceFolder);
        void SaveAdjustments(IFolder sourceFolder, IList<IPostingAdjustment> postingAdjustments);
    }
}
