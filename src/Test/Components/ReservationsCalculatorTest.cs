using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class ReservationsCalculatorTest {
    private const string NotMatching = "NotMatching", Fix = "Fix", ALittleMore = "ALittleMore";
    private const string Negative = "-", Positive = "+";
    private const double Sum = 4711;

    private readonly List<IIrregularDebitClassification> _IrregularDebitClassifications;

    private IReservationsCalculator _Sut;

    public ReservationsCalculatorTest() {
        _IrregularDebitClassifications = new List<IIrregularDebitClassification> {
            CreateIrregularDebitClassification(Fix, 100),
            CreateIrregularDebitClassification(ALittleMore, 120)
        };
    }

    [TestInitialize]
    public void Initialize() {
        _Sut = new ReservationsCalculator();
    }

    [TestMethod]
    public void ReservationsCalculator_WithEmptyClassifications_Returns0() {
        var classification = CreateClassification(Negative, "");
        var result = _Sut.Calculate(classification, Sum, new List<IIrregularDebitClassification>());
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithoutMatchingClassifications_Returns0() {
        var classification = CreateClassification(Negative, NotMatching);
        var result = _Sut.Calculate(classification, Sum, _IrregularDebitClassifications);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithFixClassificationAndNegativeSum_ReturnsSum() {
        var classification = CreateClassification(Negative, Fix);
        var result = _Sut.Calculate(classification, Sum, _IrregularDebitClassifications);
        Assert.AreEqual(-Sum, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithALittleMoreClassificationAndNegativeSum_ReturnsALittleMore() {
        var classification = CreateClassification(Negative, ALittleMore);
        var result = _Sut.Calculate(classification, Sum, _IrregularDebitClassifications);
        var expectedResult = -Math.Ceiling(Sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void ReservationsCalculator_WithFixClassificationAndPositiveSum_Returns0() {
        var classification = CreateClassification(Positive, Fix);
        var result = _Sut.Calculate(classification, Sum, _IrregularDebitClassifications);
        Assert.AreEqual(0, result);
    }

    private static IFormattedClassification CreateClassification(string sign, string classification) {
        return new FormattedClassification { Sign = sign, Classification = classification };
    }

    private static IIrregularDebitClassification CreateIrregularDebitClassification(string classification, int percentage) {
        return new IrregularDebitClassification { Classification = classification, Percentage = percentage };
    }
}