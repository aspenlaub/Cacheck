using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class CalculationLogger : ICalculationLogger {
    private readonly IFolder CalculationLogFolder;
    private readonly Dictionary<string, List<ClassificationContribution>> ContributionsPerClassification;

    public CalculationLogger(ILogConfiguration logConfiguration) {
        CalculationLogFolder = new Folder(Path.GetTempPath()).SubFolder(logConfiguration.LogSubFolder).SubFolder(nameof(CalculationLogger));
        ContributionsPerClassification = new Dictionary<string, List<ClassificationContribution>>();
    }

    public void ClearLogs() {
        CalculationLogFolder.CreateIfNecessary();
        foreach (var fileName in Directory.GetFiles(CalculationLogFolder.FullName, "*.log")) {
            File.Delete(fileName);
        }
    }

    public void RegisterContribution(string classification, double amount, IPosting posting) {
        if (!ContributionsPerClassification.ContainsKey(classification)) {
            ContributionsPerClassification[classification] = new List<ClassificationContribution>();
        }

        ContributionsPerClassification[classification].Add(new ClassificationContribution { Amount = amount, Posting = posting });
    }

    public void Flush() {
        foreach (var classification in ContributionsPerClassification.Keys) {
            var contributions = ContributionsPerClassification[classification].OrderByDescending(c => Math.Abs(c.Amount)).ToList();
            double sum = 0;
            var contents = "";
            foreach (var contribution in contributions) {
                sum = sum + contribution.Amount;
                contents = contents + "\r\n" + contribution.Amount + " (" + contribution.Posting + "; new balance " + Math.Ceiling(sum) + ")";
            }

            var fileName = CalculationLogFolder.FullName + @"\" + classification + ".log";
            if (!File.Exists(fileName)) {
                File.WriteAllText(fileName, contents);
            } else {
                File.AppendAllText(fileName, contents);
            }
            ContributionsPerClassification[classification] = new List<ClassificationContribution>();
        }
    }
}

internal class ClassificationContribution {
    public double Amount { get; init; }
    public IPosting Posting { get; init; }
}