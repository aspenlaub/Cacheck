using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class ReservationsCalculatorTest {
    private const string _notMatching = "NotMatching", _fix = "Fix", _aLittleMore = "ALittleMore";
    private const string _negative = "-", _positive = "+";
    private const double _sum = 4711;

    private readonly List<IIrregularDebitClassification> _IrregularDebitClassifications = [
        CreateIrregularDebitClassification(_fix, 100),
        CreateIrregularDebitClassification(_aLittleMore, 120)
    ];

    private IReservationsCalculator _Sut;

    [TestInitialize]
    public void Initialize() {
        _Sut = new ReservationsCalculator();
    }

    [TestMethod]
    public void ReservationsCalculator_WithEmptyClassifications_Returns0() {
        IFormattedClassification classification = CreateClassification(_negative, "");
        double result = _Sut.Calculate(classification, _sum, new List<IIrregularDebitClassification>());
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithoutMatchingClassifications_Returns0() {
        IFormattedClassification classification = CreateClassification(_negative, _notMatching);
        double result = _Sut.Calculate(classification, _sum, _IrregularDebitClassifications);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithFixClassificationAndNegativeSum_ReturnsSum() {
        IFormattedClassification classification = CreateClassification(_negative, _fix);
        double result = _Sut.Calculate(classification, _sum, _IrregularDebitClassifications);
        Assert.AreEqual(_sum, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithALittleMoreClassificationAndNegativeSum_ReturnsALittleMore() {
        IFormattedClassification classification = CreateClassification(_negative, _aLittleMore);
        double result = _Sut.Calculate(classification, _sum, _IrregularDebitClassifications);
        double expectedResult = Math.Ceiling(_sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithFixClassificationAndPositiveSum_Returns0() {
        IFormattedClassification classification = CreateClassification(_positive, _fix);
        double result = _Sut.Calculate(classification, _sum, _IrregularDebitClassifications);
        Assert.AreEqual(0, result);
    }

    private static IFormattedClassification CreateClassification(string sign, string classification) {
        return new FormattedClassification { Sign = sign, Classification = classification };
    }

    private static IIrregularDebitClassification CreateIrregularDebitClassification(string classification, int percentage) {
        return new IrregularDebitClassification { Classification = classification, Percentage = percentage };
    }
}