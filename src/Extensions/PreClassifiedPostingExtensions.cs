using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;

public static class PreClassifiedPostingExtensions {
    extension(IPreClassifiedPosting preClassifiedPosting) {
        public IClassifiedPosting FromDto() {
            return new ClassifiedPosting {
                Amount = preClassifiedPosting.Amount,
                Classification = preClassifiedPosting.Classification,
                Date = preClassifiedPosting.Date,
                Guid = preClassifiedPosting.Guid,
                Ineliminable = preClassifiedPosting.Ineliminable,
                IsIndividual = preClassifiedPosting.IsIndividual,
                Unfair = preClassifiedPosting.Unfair
            };
        }
    }
}
