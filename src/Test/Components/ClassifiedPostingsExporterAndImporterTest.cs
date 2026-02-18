using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Seoa.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Skladasu.Entities;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class ClassifiedPostingsExporterAndImporterTest {
    private IList<IPosting> _AllTimePostings;
    private IList<IClassifiedPosting> _ClassifiedPostings;
    private IClassifiedPostingsCalculator _Calculator;
#pragma warning disable CA1859
    private IClassifiedPostingsExporter _ExportSut;
    private IClassifiedPostingsImporter _ImportSut;
#pragma warning restore CA1859

    [TestInitialize]
    public async Task Initialize() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null)).Build();
        IDataCollector dataCollector = container.Resolve<IDataCollector>();
        IPostingCollector postingCollector = container.Resolve<IPostingCollector>();
        _Calculator = container.Resolve<IClassifiedPostingsCalculator>();
        _AllTimePostings = await postingCollector.CollectPostingsAsync(false);
        var errorsAndInfos = new ErrorsAndInfos();
        List<IPostingClassification> postingClassifications = await dataCollector.CollectPostingClassificationsAsync(errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        _ClassifiedPostings = await _Calculator.CalculateAndShowClassifiedPostingsAsync(_AllTimePostings,
            postingClassifications, DateTime.MinValue, 0, "", "");
        _ExportSut = new ClassifiedPostingsExporter();
        _ImportSut = new ClassifiedPostingsImporter();
    }

    [TestMethod]
    public async Task CanExportClassifiedPostings() {
        if (_AllTimePostings.AreAllPostingsPreClassified()) { return; }

        string exportFileFullName = ExportToFileReturnName(_ClassifiedPostings);
        string json = await File.ReadAllTextAsync(exportFileFullName, TestContext.CancellationToken);
        List<ClassifiedPostingDto> deserializedPostings = JsonSerializer.Deserialize<List<ClassifiedPostingDto>>(json);
        Assert.HasCount(_AllTimePostings.Count, deserializedPostings);
    }

    [TestMethod]
    public async Task CanImportClassifiedPostings() {
        if (_AllTimePostings.AreAllPostingsPreClassified()) { return; }

        string exportFileFullName = ExportToFileReturnName(_ClassifiedPostings);
        var errorsAndInfos = new ErrorsAndInfos();
        IList<IPosting> importedPostings = await _ImportSut.ImportClassifiedPostingsAsync(exportFileFullName, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.HasCount(_ClassifiedPostings.Count, importedPostings);
        IList<IClassifiedPosting> classifiedPostings
            = await _Calculator.CalculateAndShowClassifiedPostingsAsync(importedPostings, [], DateTime.MinValue, 0, "", "");
        Assert.HasCount(classifiedPostings.Count, _ClassifiedPostings);
        foreach (IClassifiedPosting expectedClassifiedPosting in _ClassifiedPostings) {
            IClassifiedPosting actualClassifiedPosting = classifiedPostings.SingleOrDefault(p => p.Guid == expectedClassifiedPosting.Guid);
            Assert.IsNotNull(actualClassifiedPosting, $"Classified posting {expectedClassifiedPosting.Guid} not found");
            Assert.AreEqual(expectedClassifiedPosting.Amount, actualClassifiedPosting.Amount);
            Assert.AreEqual(expectedClassifiedPosting.Classification, actualClassifiedPosting.Classification);
            Assert.AreEqual(expectedClassifiedPosting.Date, actualClassifiedPosting.Date);
            Assert.AreEqual(expectedClassifiedPosting.Ineliminable, actualClassifiedPosting.Ineliminable);
            Assert.AreEqual(expectedClassifiedPosting.IsIndividual, actualClassifiedPosting.IsIndividual);
        }
    }

    [TestMethod]
    public async Task SummaryIsIdenticalForImportedClassifiedPostings() {
        if (_AllTimePostings.AreAllPostingsPreClassified()) { return; }

        string exportFileFullName = ExportToFileReturnName(_ClassifiedPostings);
        var errorsAndInfos = new ErrorsAndInfos();
        IList<IPosting> importedPostings = await _ImportSut.ImportClassifiedPostingsAsync(exportFileFullName, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.HasCount(_ClassifiedPostings.Count, importedPostings);

        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null)).Build();
        var fakeDataPresenter = container.Resolve<IDataPresenter>() as FakeDataPresenter;
        Assert.IsNotNull(fakeDataPresenter);
        IDataCollector dataCollector = container.Resolve<IDataCollector>();
        ISummaryCalculator summaryCalculator = container.Resolve<ISummaryCalculator>();
        List<IPostingClassification> postingClassifications = await dataCollector.CollectPostingClassificationsAsync(errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        List<IInverseClassificationPair> inverseClassifications = await dataCollector.CollectInverseClassifications(errorsAndInfos);

        Assert.IsTrue(await summaryCalculator.CalculateAndShowSummaryAsync(_AllTimePostings, postingClassifications, inverseClassifications));
        var expectedOverallSums = fakeDataPresenter
          .OverallSums
          .Select(x => new TypeItemSum { Item = x.Item, Sum = x.Sum })
          .OrderBy(x => x.Item)
          .ToList();
        Assert.IsTrue(await summaryCalculator.CalculateAndShowSummaryAsync(importedPostings, postingClassifications, inverseClassifications));
        List<ITypeItemSum> actualOverallSums = fakeDataPresenter
            .OverallSums
            .OrderBy(x => x.Item)
            .ToList();
        Assert.HasCount(expectedOverallSums.Count, actualOverallSums);
        for (int i = 0; i < expectedOverallSums.Count; i++) {
            ITypeItemSum expectedOverallSum = expectedOverallSums[i];
            ITypeItemSum actualOverallSum = actualOverallSums[i];
            Assert.AreEqual(expectedOverallSum.Item, actualOverallSum.Item);
            Assert.IsLessThan(0.01, Math.Abs(expectedOverallSum.Sum - actualOverallSum.Sum));
        }
    }

    private string ExportToFileReturnName(IList<IClassifiedPosting> classifiedPostings) {
        IFolder folder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubTemp").SubFolder(nameof(ClassifiedPostingsExporter));
        folder.CreateIfNecessary();
        string exportFileFullName = folder.FullName + @"\postings.json";
        if (File.Exists(exportFileFullName)) {
            File.Delete(exportFileFullName);
        }
        _ExportSut.ExportClassifiedPostings(exportFileFullName, classifiedPostings);
        Assert.IsTrue(File.Exists(exportFileFullName));
        return exportFileFullName;
    }

    public TestContext TestContext { get; set; }
}