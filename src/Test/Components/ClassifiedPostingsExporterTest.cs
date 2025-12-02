using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class ClassifiedPostingsExporterTest {
    [TestMethod]
    public async Task CanExportClassifiedPostings() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null)).Build();
        IDataCollector dataCollector = container.Resolve<IDataCollector>();
        IPostingCollector postingCollector = container.Resolve<IPostingCollector>();
        IClassifiedPostingsCalculator calculator = container.Resolve<IClassifiedPostingsCalculator>();
        IList<IPosting> allTimePostings = await postingCollector.CollectPostingsAsync(false);
        var errorsAndInfos = new ErrorsAndInfos();
        List<IPostingClassification> postingClassifications = await dataCollector.CollectPostingClassificationsAsync(errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        IList<IClassifiedPosting> classifiedPostings = await calculator.CalculateAndShowClassifiedPostingsAsync(allTimePostings,
            postingClassifications, DateTime.MinValue, 0, "", "");
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
        IClassifiedPostingsExporter sut = new ClassifiedPostingsExporter();
#pragma warning restore CA1859 // Use concrete types when possible for improved performance
        IFolder folder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubTemp").SubFolder(nameof(ClassifiedPostingsExporter));
        folder.CreateIfNecessary();
        string exportFileFullName = folder.FullName + @"\postings.json";
        if (File.Exists(exportFileFullName)) {
            File.Delete(exportFileFullName);
        }
        sut.ExportClassifiedPostings(exportFileFullName, classifiedPostings);
        Assert.IsTrue(File.Exists(exportFileFullName));
        string json = await File.ReadAllTextAsync(exportFileFullName, TestContext.CancellationToken);
        List<ClassifiedPostingDto> deserializedPostings = JsonSerializer.Deserialize<List<ClassifiedPostingDto>>(json);
        Assert.HasCount(allTimePostings.Count, deserializedPostings);
    }

    public TestContext TestContext { get; set; }
}
