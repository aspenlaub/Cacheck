using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class IndividualPostingClassificationsSourceTest {
    private IIndividualPostingClassificationsSource _Sut;

    [TestInitialize]
    public async Task Initialize() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        _Sut = container.Resolve<IIndividualPostingClassificationsSource>();
    }

    [TestMethod]
    public async Task CanGetIndividualPostingClassifications() {
        var errorsAndInfos = new ErrorsAndInfos();
        var individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        individualPostingClassifications = individualPostingClassifications.Where(i => i.PostingHash != nameof(OneIndividualPostingClassificationExists)).ToList();
        Assert.IsTrue(!individualPostingClassifications.Any(),
            "Individual posting classifications have changed, please adjust this test");
    }

    [TestMethod]
    public async Task PostingExistsForEachIndividualPostingClassification() {
        var errorsAndInfos = new ErrorsAndInfos();
        var individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        individualPostingClassifications = individualPostingClassifications.Where(i => i.PostingHash != nameof(OneIndividualPostingClassificationExists)).ToList();
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(!individualPostingClassifications.Any(),
            "Individual posting classifications have changed, please adjust this test");
    }

    [TestMethod]
    public async Task OneIndividualPostingClassificationExists() {
        var individualPostingClassification = new IndividualPostingClassification {
            Classification = nameof(IndividualPostingClassificationsSourceTest),
            PostingHash = nameof(OneIndividualPostingClassificationExists),
            Credit = false
        };
        var errorsAndInfos = new ErrorsAndInfos();
        var individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash)) { return; }

        await _Sut.AddAsync(individualPostingClassification, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash),
            "Individual posting classification could not be added");
    }

    [TestMethod]
    public async Task CanAddIndividualPostingClassifications() {
        var individualPostingClassification = new IndividualPostingClassification {
            Classification = nameof(IndividualPostingClassificationsSourceTest),
            PostingHash = nameof(CanAddIndividualPostingClassifications),
            Credit = false
        };
        var errorsAndInfos = new ErrorsAndInfos();
        var individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        if (individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash)) {
            await _Sut.RemoveAsync(individualPostingClassification);
            individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            Assert.IsFalse(individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash),
                "Individual posting classification could not be removed");
        }

        await _Sut.AddAsync(individualPostingClassification, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsTrue(individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash),
            "Individual posting classification could not be added");

        await _Sut.RemoveAsync(individualPostingClassification);
        individualPostingClassifications = await _Sut.GetAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.IsFalse(individualPostingClassifications.Any(i => i.PostingHash == individualPostingClassification.PostingHash),
            "Individual posting classification could not be removed");
    }
}