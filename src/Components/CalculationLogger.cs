using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;

public class CalculationLogger(ILogConfiguration logConfiguration) : ICalculationLogger {

    private readonly IFolder _CalculationLogFolder = new Folder(Path.GetTempPath()).SubFolder(logConfiguration.LogSubFolder).SubFolder(nameof(CalculationLogger));
    private readonly Dictionary<string, List<ClassificationContribution>> _ContributionsPerClassification = new();

    public void ClearLogs() {
        _CalculationLogFolder.CreateIfNecessary();
        foreach (string fileName in Directory.GetFiles(_CalculationLogFolder.FullName, "*.log")) {
            File.Delete(fileName);
        }
    }

    public void RegisterContribution(string classification, double amount, IPosting posting) {
        if (!_ContributionsPerClassification.ContainsKey(classification)) {
            _ContributionsPerClassification[classification] = [];
        }

        _ContributionsPerClassification[classification].Add(new ClassificationContribution { Amount = amount, Posting = posting });
    }

    public void Flush() {
        foreach (string classification in _ContributionsPerClassification.Keys) {
            var contributions = _ContributionsPerClassification[classification].OrderByDescending(c => Math.Abs(c.Amount)).ToList();
            double sum = 0;
            string contents = "";
            foreach (ClassificationContribution contribution in contributions) {
                sum = sum + contribution.Amount;
                contents = contents + "\r\n" + contribution.Amount + " (" + contribution.Posting + "; new balance " + Math.Ceiling(sum) + ")";
            }

            string fileName = _CalculationLogFolder.FullName + @"\" + classification + ".log";
            if (!File.Exists(fileName)) {
                File.WriteAllText(fileName, contents);
            } else {
                File.AppendAllText(fileName, contents);
            }
            _ContributionsPerClassification[classification] = [];
        }
    }
}

internal class ClassificationContribution {
    public double Amount { get; init; }
    public IPosting Posting { get; init; }
}