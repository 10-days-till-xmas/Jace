using System;
using System.Globalization;
using System.Linq.Expressions;
using Xunit;

namespace Jace.Tests;

public abstract partial class CalculationEngineTests
{
    [Theory]
    [Trait("Category", "Other")]
    [InlineData("2+3", 5.0)]
    [InlineData("2.0+3.0", 5.0)]
    public void TestCalculateFormula1(string formula, double expected)
    {
        var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
        var result = engine.Calculate(formula);

        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Other")]
    public void TestPiMultiplication()
    {
        var engine = new CalculationEngine();
        var result = engine.Calculate("2 * pI");

        Assert.Equal(2 * Math.PI, result);
    }

    [Theory]
    [Trait("Category", "Other")]
    [InlineData("1+2-3*4/5+6-7*8/9+0", 0.378)]
    [InlineData("1+2-3*4/sqrt(25)+6-7*8/9+0", 0.378)]
    public void TestComplicatedPrecedence(string formula, double expected)
    {
        var engine = new CalculationEngine();

        var result = engine.Calculate(formula);
        Assert.Equal(expected, Math.Round(result, 3));
    }

    [Theory]
    [Trait("Category", "Other")]
    [InlineData("ifless(0.57, (3000-500)/(1500-500), 10, 20)", 10)]
    [InlineData("if(0.57 < (3000-500)/(1500-500), 10, 20)", 10)]
    public void TestExpressionArguments(string formula, double expected)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate(formula);
        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Other")]
    public void TestCalculationCompiledExpression()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = true
        });
        Expression<Func<double, double, double>> expression = (a, b) => a + b;
        expression.Compile();

        engine.AddFunction("test", expression.Compile());

        var result = engine.Calculate("test(2, 3)");
        Assert.Equal(5.0, result);
    }
}
