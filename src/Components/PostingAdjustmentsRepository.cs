using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components {
    public class PostingAdjustmentsRepository : IPostingAdjustmentsRepository {
        private readonly IXmlSerializer XmlSerializer;
        private readonly IXmlDeserializer XmlDeserializer;

        public PostingAdjustmentsRepository(IXmlSerializer xmlSerializer, IXmlDeserializer xmlDeserializer) {
            XmlSerializer = xmlSerializer;
            XmlDeserializer = xmlDeserializer;
        }

        public IList<IPostingAdjustment> LoadAdjustments(IFolder sourceFolder) {
            var fileName = sourceFolder.FullName + @"\PostingAdjustments.xml";
            if (!File.Exists(fileName)) {
                return new List<IPostingAdjustment>();
            }

            var xml = File.ReadAllText(fileName);
            var adjustments = XmlDeserializer.Deserialize<PostingAdjustments>(xml).Cast<IPostingAdjustment>().ToList();
            return adjustments;
        }

        public void SaveAdjustments(IFolder sourceFolder, IList<IPostingAdjustment> postingAdjustments) {
            var xml = XmlSerializer.Serialize(new PostingAdjustments(postingAdjustments));
            var fileName = sourceFolder.FullName + @"\PostingAdjustments.xml";
            if (File.Exists(fileName) && File.ReadAllText(fileName) == xml) { return; }

            File.WriteAllText(fileName, xml);
        }
    }
}
