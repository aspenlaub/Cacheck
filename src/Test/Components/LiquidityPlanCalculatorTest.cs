using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class LiquidityPlanCalculatorTest {
    private const string NotMatching = "NotMatching", Fix = "Fix", ALittleMore = "ALittleMore", Target = "Target";
    private const string Negative = "-", Positive = "+";
    private const double Sum = 4711, TheTarget = 1147;

    private readonly List<ILiquidityPlanClassification> _LiquidityPlanClassifications;

    private ILiquidityPlanCalculator _Sut;

    public LiquidityPlanCalculatorTest() {
        _LiquidityPlanClassifications = new List<ILiquidityPlanClassification> {
            CreateLiquidityPlanClassification(Fix, 100, 0),
            CreateLiquidityPlanClassification(ALittleMore, 120, 0),
            CreateLiquidityPlanClassification(Target, 0, TheTarget)
        };
    }

    [TestInitialize]
    public void Initialize() {
        _Sut = new LiquidityPlanCalculator();
    }

    [TestMethod]
    public void LiquidityPlanCalculator_WithEmptyClassifications_Returns0() {
        var classification = CreateClassification(Negative, "");
        var result = _Sut.Calculate(classification, Sum, new List<ILiquidityPlanClassification>());
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithoutMatchingClassifications_Returns0() {
        var classification = CreateClassification(Negative, NotMatching);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithFixClassificationsAndPositiveSum_ReturnsSum() {
        var classification = CreateClassification(Positive, Fix);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        Assert.AreEqual(Sum, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithFixClassificationsAndNegativeSum_ReturnsSum() {
        var classification = CreateClassification(Negative, Fix);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        Assert.AreEqual(-Sum, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithALittleMoreClassificationsAndPositiveSum_ReturnsALittleMore() {
        var classification = CreateClassification(Positive, ALittleMore);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        var expectedResult = Math.Ceiling(Sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithALittleMoreClassificationsAndNegativeSum_ReturnsALittleMore() {
        var classification = CreateClassification(Negative, ALittleMore);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        var expectedResult = -Math.Ceiling(Sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithTargetClassificationsAndPositiveSum_ReturnsTarget() {
        var classification = CreateClassification(Positive, Target);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        Assert.AreEqual(TheTarget, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithTargetClassificationsAndNegativeSum_ReturnsNegativeTarget() {
        var classification = CreateClassification(Negative, Target);
        var result = _Sut.Calculate(classification, Sum, _LiquidityPlanClassifications);
        Assert.AreEqual(-TheTarget, result);
    }

    private static IFormattedClassification CreateClassification(string sign, string classification) {
        return new FormattedClassification { Sign = sign, Classification = classification };
    }

    private ILiquidityPlanClassification CreateLiquidityPlanClassification(string classification, int percentage, double target) {
        return new LiquidityPlanClassification { Classification = classification, Percentage = percentage, Target = target };
    }
}