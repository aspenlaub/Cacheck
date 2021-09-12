using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components {
    [TestClass]
    public class PostingAdjustmentsRepositoryTest {
        private readonly IFolder SourceFolder = new Folder(Path.GetTempPath()).SubFolder("AspenlaubTemp").SubFolder(nameof(PostingAdjustmentsRepositoryTest));

        private IContainer Container;
        private IPostingAdjustmentsRepository Sut;

        [TestInitialize]
        public void Initialize() {
            Container = new ContainerBuilder().UseCacheckVishizhukelNetAndPeghWithFakesAsync(null).Result.Build();

            Cleanup();
            SourceFolder.CreateIfNecessary();
            Sut = Container.Resolve<IPostingAdjustmentsRepository>();
        }

        [TestCleanup]
        public void Cleanup() {
            if (!SourceFolder.Exists()) { return; }

            var deleter = Container.Resolve<IFolderDeleter>();
            deleter.DeleteFolder(SourceFolder);
        }

        [TestMethod]
        public void CanSaveEmptyAdjustments() {
            var adjustments = new List<IPostingAdjustment>();
            Sut.SaveAdjustments(SourceFolder, adjustments);
            var loadedAdjustments = Sut.LoadAdjustments(SourceFolder);
            Assert.IsTrue(Enumerable.SequenceEqual(adjustments, loadedAdjustments));
        }

        [TestMethod]
        public void CanSaveAdjustment() {
            var adjustments = new List<IPostingAdjustment> {
                CreateAdjustment(new DateTime(2021, 9, 11), "NoClue", 4711, 4711.11)
            };
            Sut.SaveAdjustments(SourceFolder, adjustments);
            var loadedAdjustments = Sut.LoadAdjustments(SourceFolder);
            Assert.IsTrue(Enumerable.SequenceEqual(adjustments, loadedAdjustments));
        }

        [TestMethod]
        public void AdjustmentsAreSortedWhenSaved() {
            var adjustments = new List<IPostingAdjustment> {
                CreateAdjustment(new DateTime(2021, 9, 11), "NoClue", 4711, 4711.11),
                CreateAdjustment(new DateTime(2021, 9, 10), "NoClue", 4711, 4711.11),
                CreateAdjustment(new DateTime(2021, 9, 11), "AClue", 4711, 4711.11),
            };
            Sut.SaveAdjustments(SourceFolder, adjustments);
            var loadedAdjustments = Sut.LoadAdjustments(SourceFolder);
            adjustments = adjustments.OrderByDescending(a => a.Date).ToList();
            Assert.IsTrue(Enumerable.SequenceEqual(adjustments, loadedAdjustments));
        }

        private IPostingAdjustment CreateAdjustment(DateTime date, string clue, double amount, double adjustedAmount) {
            return new PostingAdjustment {
                Date = date, Clue = clue, Amount = amount, AdjustedAmount = adjustedAmount
            };
        }
    }
}
