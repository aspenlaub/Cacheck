using System;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Components;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Entities;
using Aspenlaub.Net.GitHub.CSharp.Cacheck.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Aspenlaub.Net.GitHub.CSharp.Cacheck.Test.Components;

[TestClass]
public class LiquidityPlanCalculatorTest {
    private const string _notMatching = "NotMatching", _fix = "Fix", _aLittleMore = "ALittleMore", _target = "Target";
    private const string _negative = "-", _positive = "+";
    private const double _sum = 4711, _theTarget = 1147;

    private readonly List<ILiquidityPlanClassification> _LiquidityPlanClassifications;

    private ILiquidityPlanCalculator _Sut;

    public LiquidityPlanCalculatorTest() {
        _LiquidityPlanClassifications = [
            CreateLiquidityPlanClassification(_fix, 100, 0),
            CreateLiquidityPlanClassification(_aLittleMore, 120, 0),
            CreateLiquidityPlanClassification(_target, 0, _theTarget)
        ];
    }

    [TestInitialize]
    public void Initialize() {
        _Sut = new LiquidityPlanCalculator();
    }

    [TestMethod]
    public void LiquidityPlanCalculator_WithEmptyClassifications_Returns0() {
        IFormattedClassification classification = CreateClassification(_negative, "");
        double result = _Sut.Calculate(classification, _sum, new List<ILiquidityPlanClassification>());
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithoutMatchingClassifications_Returns0() {
        IFormattedClassification classification = CreateClassification(_negative, _notMatching);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithFixClassificationsAndPositiveSum_ReturnsSum() {
        IFormattedClassification classification = CreateClassification(_positive, _fix);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        Assert.AreEqual(_sum, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithFixClassificationsAndNegativeSum_ReturnsSum() {
        IFormattedClassification classification = CreateClassification(_negative, _fix);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        Assert.AreEqual(-_sum, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithALittleMoreClassificationsAndPositiveSum_ReturnsALittleMore() {
        IFormattedClassification classification = CreateClassification(_positive, _aLittleMore);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        double expectedResult = Math.Ceiling(_sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithALittleMoreClassificationsAndNegativeSum_ReturnsALittleMore() {
        IFormattedClassification classification = CreateClassification(_negative, _aLittleMore);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        double expectedResult = -Math.Ceiling(_sum * 120 / 100);
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithTargetClassificationsAndPositiveSum_ReturnsTarget() {
        IFormattedClassification classification = CreateClassification(_positive, _target);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        Assert.AreEqual(_theTarget, result);
    }

    [TestMethod]
    public void LiquidityPlanCalculatorCalculator_WithTargetClassificationsAndNegativeSum_ReturnsNegativeTarget() {
        IFormattedClassification classification = CreateClassification(_negative, _target);
        double result = _Sut.Calculate(classification, _sum, _LiquidityPlanClassifications);
        Assert.AreEqual(-_theTarget, result);
    }

    private static IFormattedClassification CreateClassification(string sign, string classification) {
        return new FormattedClassification { Sign = sign, Classification = classification };
    }

    private ILiquidityPlanClassification CreateLiquidityPlanClassification(string classification, int percentage, double target) {
        return new LiquidityPlanClassification { Classification = classification, Percentage = percentage, Target = target };
    }
}