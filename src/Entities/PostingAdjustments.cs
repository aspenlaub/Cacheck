using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities {
    [XmlRoot("PostingAdjustments", Namespace = "http://www.aspenlaub.net")]
    public class PostingAdjustments : List<PostingAdjustment> {
        public PostingAdjustments() {
        }

        public PostingAdjustments(IList<IPostingAdjustment> adjustments) : base(adjustments.Select(p => new PostingAdjustment(p)).OrderBy(a => a.Date).ThenBy(a => a.Clue).ToList()) {
        }
    }
}
