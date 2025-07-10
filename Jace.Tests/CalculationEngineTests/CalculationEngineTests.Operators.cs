using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Jace.Tests;

public abstract partial class CalculationEngineTests
{
    [Fact]
    [Trait("Category", "Operators")]
    public void TestCalculateModulo()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5 % 3.0");

        Assert.Equal(2.0, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestCalculatePow()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("2^3.0");

        Assert.Equal(8.0, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestLessThan()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 4.2 }
        };

        var result = engine.Calculate("var1 < var2", variables);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [Trait("Category", "Operators")]
    [InlineData("var1 <= var2", 1.0)]
    [InlineData("var1 ≤ var2", 1.0)]
    public void TestLessOrEqualThan1(string formula, double expected)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate(formula, variables);
        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestGreaterThan()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 3 }
        };

        var result = engine.Calculate("var1 > var2", variables);
        Assert.Equal(0.0, result);
    }

    [Theory]
    [Trait("Category", "Operators")]
    [InlineData("var1 >= var2", 1.0)]
    [InlineData("var1 ≥ var2", 1.0)]
    public void TestGreaterOrEqualThan1(string formula, double expected)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate(formula, variables);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Operators")]
    [InlineData("var1 != 2", 0.0)]
    [InlineData("var1 ≠ 2", 0.0)]
    public void TestNotEqual(string formula, double expected)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate(formula, variables);
        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestEqual()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 == 2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestAnd()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("(1 && 0)");
        Assert.Equal(0, result);
    }

    [Theory]
    [Trait("Category", "Operators")]
    [InlineData("(1 || 0)", 1)]
    [InlineData("(0 || 0)", 0)]
    public void TestOr1(string formula, double expected)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate(formula);
        Assert.Equal(expected, result);
    }

    [Theory]
    [Trait("Category", "Operators")]
    [InlineData("-(1+2+(3+4))", -10.0)]
    [InlineData("5+(-(1*2))", 3.0)]
    [InlineData("5*(-(1*2)*3)", -30.0)]
    [InlineData("5* -(1*2)", -10.0)]
    [InlineData("-(1*2)^3", -8.0)]
    public void TestUnaryMinus(string formula, double expected)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate(formula);

        Assert.Equal(expected, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestNegativeConstant()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("-100");

        Assert.Equal(-100.0, result);
    }

    [Fact]
    [Trait("Category", "Operators")]
    public void TestMultiplicationWithNegativeConstant()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5*-100");

        Assert.Equal(-500.0, result);
    }
}
