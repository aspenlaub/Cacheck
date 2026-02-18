using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
public class SourceFileReaderTest {
    protected static DateTime DateOne => new(2020, 11, 14);
    protected static DateTime DateTwo => new(2020, 11, 15);
    protected const double AmountOne = 2470.19, AmountTwo = -1970.24;
    protected const string RemarkOne = "This Is Not A Remark", RemarkTwo = "This Is Not Remarkable";

    private static string _sampleSourceFileName;

    [TestMethod]
    public async Task CanReadPostings() {
        CreateSampleSourceFile(CreatePostingText);

        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISourceFileReader sut = container.Resolve<ISourceFileReader>();
        var errorsAndInfos = new ErrorsAndInfos();
        IList<IPosting> postings = sut.ReadPostings(_sampleSourceFileName, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.HasCount(2, postings);
        Assert.AreEqual(DateOne, postings[0].Date);
        Assert.AreEqual(AmountOne, postings[0].Amount);
        Assert.AreEqual(RemarkOne, postings[0].Remark);
        Assert.AreEqual(DateTwo, postings[1].Date);
        Assert.AreEqual(AmountTwo, postings[1].Amount);
        Assert.AreEqual(RemarkTwo, postings[1].Remark);
    }

    [TestMethod]
    public async Task CanReadBefore2025Postings() {
        CreateSampleSourceFile(CreateBefore2025PostingText);

        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        ISourceFileReader sut = container.Resolve<ISourceFileReader>();
        var errorsAndInfos = new ErrorsAndInfos();
        IList<IPosting> postings = sut.ReadPostings(_sampleSourceFileName, errorsAndInfos);
        Assert.That.ThereWereNoErrors(errorsAndInfos);
        Assert.HasCount(2, postings);
        Assert.AreEqual(DateOne, postings[0].Date);
        Assert.AreEqual(AmountOne, postings[0].Amount);
        Assert.AreEqual(RemarkOne, postings[0].Remark);
        Assert.AreEqual(DateTwo, postings[1].Date);
        Assert.AreEqual(AmountTwo, postings[1].Amount);
        Assert.AreEqual(RemarkTwo, postings[1].Remark);
    }

    protected static void CreateSampleSourceFile(Func<DateTime, double, string, string> createPostingText) {
        IFolder folder = new Folder(Path.GetTempPath()).SubFolder(nameof(SourceFileReaderTest));
        folder.CreateIfNecessary();
        string[] contents = [
            "vom " + DateOne.ToShortDateString(),
            createPostingText(DateOne, AmountOne, RemarkOne),
            createPostingText(DateTwo, AmountTwo, RemarkTwo)
        ];
        _sampleSourceFileName = folder.FullName + "\\Sample.txt";
        File.WriteAllLines(_sampleSourceFileName, contents);
    }

    protected static string CreateBefore2025PostingText(DateTime date, double amount, string remark) {
        return "  " + date.ToShortDateString() + remark + date.ToShortDateString() + amount;
    }

    protected static string CreatePostingText(DateTime date, double amount, string remark) {
        return "    " + date.ToString("dd.MM.") + "  " + date.ToString("dd.MM.") + "   " + remark
               + "    " + Math.Abs(amount).ToString("F") + " " + (amount >= 0 ? "H" : "S");
    }
}