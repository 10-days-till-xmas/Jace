using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Jace.Tests;

public abstract partial class CalculationEngineTests
{
    [Fact]
    [Trait("Category", "Variables")]
    public void TestCalculateFormulaWithVariables()
    {
        var variables = new Dictionary<string, double>
        {
            { "var1", 2.5 },
            { "var2", 3.4 }
        };
        var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
        var result = engine.Calculate("var1*var2", variables);

        Assert.Equal(8.5, result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestCalculateFormulaWithCaseSensitiveVariables1()
    {
        var variables = new Dictionary<string, double>
        {
            { "VaR1", 2.5 },
            { "vAr2", 3.4 },
            { "var1", 1 },
            { "var2", 1 }
        };

        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        var result = engine.Calculate("VaR1*vAr2", variables);

        Assert.Equal(8.5, result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestCalculateFormulaVariableNotDefined()
    {
        var variables = new Dictionary<string, double> { { "var1", 2.5 } };

        Assert.Throws<VariableNotDefinedException>(() =>
        {
            new CalculationEngine(CultureInfo.InvariantCulture, executionMode)
                .Calculate("var1*var2", variables);
        });
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestReservedVariableName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var variables = new Dictionary<string, double> { { "pi", 2.0 } };

            var engine = new CalculationEngine();
            _ = engine.Calculate("2 * pI", variables);
        });
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestVariableNameCaseSensitivity()
    {
        var variables = new Dictionary<string, double> { { "testvariable", 42.5 } };

        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("2 * tEsTVaRiaBlE", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestVariableNameCaseSensitivityNoToLower()
    {
        var variables = new Dictionary<string, double> { { "BlAbLa", 42.5 } };

        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        var result = engine.Calculate("2 * BlAbLa", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestVariableCaseFunc()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "otherVariable", 4.2 }
        };

        var result = formula(variables);
        Assert.Equal(2 + 2 / (3 * 4.2), result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestVariableCaseNonFunc()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "otherVariable", 4.2 }
        };

        var result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
        Assert.Equal(2 + 2 / (3 * 4.2), result);
    }

    [Fact]
    [Trait("Category", "Variables")]
    public void TestVariableUnderscore()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var_var_1", 1 },
            { "var_var_2", 2 }
        };

        var result = engine.Calculate("var_var_1 + var_var_2", variables);
        Assert.Equal(3.0, result);
    }
}
