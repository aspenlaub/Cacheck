using System;
using System.IO;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class SourceFileReaderTest {
    protected static DateTime DateOne => new(2020, 11, 14);
    protected static DateTime DateTwo => new(2020, 11, 15);
    protected const double AmountOne = 2470.19, AmountTwo = -1970.24;
    protected const string RemarkOne = "This Is Not A Remark", RemarkTwo = "This Is Not Remarkable";

    private static string SampleSourceFileName;

    [ClassInitialize]
    public static void Initialize(TestContext context) {
        CreateSampleSourceFile();
    }

    [TestMethod]
    public async Task CanReadPostings() {
        var container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var sut = container.Resolve<ISourceFileReader>();
        var errorsAndInfos = new ErrorsAndInfos();
        var postings = sut.ReadPostings(SampleSourceFileName, errorsAndInfos);
        Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
        Assert.AreEqual(2, postings.Count);
        Assert.AreEqual(DateOne, postings[0].Date);
        Assert.AreEqual(AmountOne, postings[0].Amount);
        Assert.AreEqual(RemarkOne, postings[0].Remark);
        Assert.AreEqual(DateTwo, postings[1].Date);
        Assert.AreEqual(AmountTwo, postings[1].Amount);
        Assert.AreEqual(RemarkTwo, postings[1].Remark);
    }

    protected static void CreateSampleSourceFile() {
        var folder = new Folder(Path.GetTempPath()).SubFolder(nameof(SourceFileReaderTest));
        folder.CreateIfNecessary();
        var contents = new[] {
            CreatePostingText(DateOne, AmountOne, RemarkOne),
            CreatePostingText(DateTwo, AmountTwo, RemarkTwo)
        };
        SampleSourceFileName = folder.FullName + "\\Sample.txt";
        File.WriteAllLines(SampleSourceFileName, contents);
    }

    protected static string CreatePostingText(DateTime date, double amount, string remark) {
        return "  " + date.ToShortDateString() + remark + date.ToShortDateString() + amount;
    }
}