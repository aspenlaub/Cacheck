using System.Collections.Generic;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class PotentialCueExtractorTest {
    private IPotentialCueExtractor _Sut;

    [TestInitialize]
    public void Initialize() {
        _Sut = new PotentialCueExtractor();
    }

    [TestMethod]
    public void CanExtractPotentialCues() {
        ExtractPotentialCues("This is not so short", new List<string> {
            "This is not so short", "This is not so", "is not so short", "not so short"
        });
        ExtractPotentialCues("This is short", new List<string> {
            "This is short"
        });
        ExtractPotentialCues("Dis is short", new List<string> {
            "Dis is short"
        });
        ExtractPotentialCues("Dis is shot", new List<string>());
    }

    private void ExtractPotentialCues(string postingRemark, ICollection<string> expectedCues) {
        ExtractPotentialCues(postingRemark, expectedCues, "", "");
        ExtractPotentialCues(postingRemark, expectedCues, "12", "");
        ExtractPotentialCues(postingRemark, expectedCues, "", "34");
        ExtractPotentialCues(postingRemark, expectedCues, "11 22 ", "");
        ExtractPotentialCues(postingRemark, expectedCues, "", " 33 44");
    }

    private void ExtractPotentialCues(string postingRemark, ICollection<string> expectedCues,
            string prefix, string postfix) {
        HashSet<string> actualCues = _Sut.ExtractPotentialCues(prefix + postingRemark + postfix);
        Assert.AreEqual(expectedCues.Count, actualCues.Count);
        Assert.IsTrue(expectedCues.All(actualCues.Contains));
    }
}