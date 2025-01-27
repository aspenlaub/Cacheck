﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Data;

[TestClass]
public class TestDataGeneratorTest {
    [TestInitialize]
    public void Initialize() {
        TestDataGenerator.ResetTestFolder();
    }

    [TestCleanup]
    public void Cleanup() {
        TestDataGenerator.RemoveTestFolder();
    }

    [TestMethod]
    public async Task CanReadPostingsFromTestFolder() {
        IContainer container = (await new ContainerBuilder().UseCacheckVishizhukelNetAndPeghAsync(null)).Build();
        var files = Directory.GetFiles(Folders.IntegrationTestFolder.FullName, "*.txt").ToList();
        ISourceFileReader reader = container.Resolve<ISourceFileReader>();
        IList<IPosting> expectedPostings = TestDataGenerator.TestPostings;
        var actualPostings = new List<IPosting>();
        var errorsAndInfos = new ErrorsAndInfos();
        foreach (IList<IPosting> postings in files.Select(file => reader.ReadPostings(file, errorsAndInfos))) {
            Assert.IsFalse(errorsAndInfos.AnyErrors(), errorsAndInfos.ErrorsToString());
            actualPostings.AddRange(postings);
        }

        Assert.AreEqual(expectedPostings.Count, actualPostings.Count);
        for (int i = 0; i < expectedPostings.Count; i++) {
            Assert.AreEqual(expectedPostings[i].Date, actualPostings[i].Date);
            Assert.AreEqual(expectedPostings[i].Amount, actualPostings[i].Amount);
            Assert.AreEqual(expectedPostings[i].Remark, actualPostings[i].Remark);
        }
    }
}