using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Jace.Execution;
using Xunit;

namespace Jace.Tests;

public class CalculationEngineTests
{
    public static readonly TheoryData<ExecutionMode> ExecutionModes =
    [
        ExecutionMode.Compiled,
        ExecutionMode.Interpreted
    ];

    [Theory]
    [InlineData(ExecutionMode.Interpreted, "2+3", 5.0)]
    [InlineData(ExecutionMode.Compiled, "2+3", 5.0)]
    [InlineData(ExecutionMode.Compiled, "2.0+3.0", 5.0)]
    public void TestCalculateFormula1(ExecutionMode mode, string formula, double expected)
    {
        var engine = new CalculationEngine(new JaceOptions{ExecutionMode = mode});
        var result = engine.Calculate(formula);

        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateModulo(ExecutionMode executionMode)
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

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculatePow(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("2^3.0");

        Assert.Equal(8.0, result);
    }

    [Fact]
    public void TestCalculateFormulaWithVariables()
    {
        var variables = new Dictionary<string, double>
        {
            { "var1", 2.5 },
            { "var2", 3.4 }
        };
        var engine = new CalculationEngine();
        var result = engine.Calculate("var1*var2", variables);

        Assert.Equal(8.5, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateFormulaWithCaseSensitiveVariables1(ExecutionMode executionMode)
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

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateFormulaVariableNotDefined(ExecutionMode executionMode)
    {
        var variables = new Dictionary<string, double> { { "var1", 2.5 } };

        Assert.Throws<VariableNotDefinedException>(() =>
        {
            new CalculationEngine(CultureInfo.InvariantCulture, executionMode)
                .Calculate("var1*var2", variables);
        });
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateSineFunctionInterpreted(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("sin(14)");

        Assert.Equal(Math.Sin(14.0), result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateCosineFunction(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("cos(41)");

        Assert.Equal(Math.Cos(41.0), result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculateLognFunction(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = false
        });
        var result = engine.Calculate("logn(14, 3)");

        Assert.Equal(Math.Log(14.0, 3.0), result);
    }

    [Fact]
    public void TestNegativeConstant()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Compiled,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("-100");

        Assert.Equal(-100.0, result);
    }

    [Fact]
    public void TestMultiplicationWithNegativeConstant()
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Compiled,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5*-100");

        Assert.Equal(-500.0, result);
    }
    
    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestUnaryMinus1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("-(1+2+(3+4))");

        Assert.Equal(-10.0, result);
    }
    
    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestUnaryMinus2(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5+(-(1*2))");

        Assert.Equal(3.0, result);
    }
    
    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestUnaryMinus3(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5*(-(1*2)*3)");

        Assert.Equal(-30.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestUnaryMinus4(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5* -(1*2)");

        Assert.Equal(-10.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestUnaryMinus5Compiled(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("-(1*2)^3");

        Assert.Equal(-8.0, result);
    }

    [Fact]
    public void TestBuild()
    {
        var engine = new CalculationEngine();
        Func<Dictionary<string, double>, double> function = engine.Build("var1+2*(3*age)");

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "age", 4 }
        };

        var result = function(variables);
        Assert.Equal(26.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestFormulaBuilder(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
            .Parameter("var1", DataType.Integer)
            .Parameter("age", DataType.FloatingPoint)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = function(2, 4);
        Assert.Equal(26.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestFormulaBuilderConstant(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        engine.AddConstant("age", 18.0);

        var function = (Func<int, double>)engine.Formula("age+var1")
            .Parameter("var1", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = function(3);
        Assert.Equal(21.0, result);
    }

    [Fact]
    public void TestFormulaBuilderInvalidParameterName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var engine = new CalculationEngine();
            _ = (Func<int, double, double>)engine.Formula("sin+2")
                .Parameter("sin", DataType.Integer)
                .Build();
        });
    }

    [Fact]
    public void TestFormulaBuilderDuplicateParameterName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var engine = new CalculationEngine();
            _ = (Func<int, double, double>)engine.Formula("var1+2")
                .Parameter("var1", DataType.Integer)
                .Parameter("var1", DataType.FloatingPoint)
                .Build();
        });
    }

    [Fact]
    public void TestPiMultiplication()
    {
        var engine = new CalculationEngine();
        var result = engine.Calculate("2 * pI");

        Assert.Equal(2 * Math.PI, result);
    }

    [Fact]
    public void TestReservedVariableName()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            var variables = new Dictionary<string, double> { { "pi", 2.0 } };

            var engine = new CalculationEngine();
            _ = engine.Calculate("2 * pI", variables);
        });
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestVariableNameCaseSensitivity(ExecutionMode executionMode)
    {
        var variables = new Dictionary<string, double> { { "testvariable", 42.5 } };

        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var result = engine.Calculate("2 * tEsTVaRiaBlE", variables);

        Assert.Equal(85.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestVariableNameCaseSensitivityNoToLower(ExecutionMode executionMode)
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

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCustomFunction(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        engine.AddFunction("test", (a, b) => a + b);

        var result = engine.Calculate("test(2,3)");
        Assert.Equal(5.0, result);
    }

    [Theory]
    [InlineData("1+2-3*4/5+6-7*8/9+0", 0.378)]
    [InlineData("1+2-3*4/sqrt(25)+6-7*8/9+0", 0.378)]
    public void TestComplicatedPrecedence(string formula, double expected)
    {
        var engine = new CalculationEngine();

        var result = engine.Calculate(formula);
        Assert.Equal(expected, Math.Round(result, 3));
    }

    [Theory]
    [InlineData("ifless(0.57, (3000-500)/(1500-500), 10, 20)", 10)]
    [InlineData("if(0.57 < (3000-500)/(1500-500), 10, 20)", 10)]
    public void TestExpressionArguments(string formula, double expected)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate(formula);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestNestedFunctions()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate("max(sin(67), cos(67))");
        Assert.Equal(-0.517769799789505, Math.Round(result, 15));
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestVariableCaseFunc(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "otherVariable", 4.2 }
        };

        var result = formula(variables);
        Assert.Equal(2+2/(3*4.2), result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestVariableCaseNonFunc(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "otherVariable", 4.2 }
        };

        var result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
        Assert.Equal(2+2/(3*4.2), result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestLessThan(ExecutionMode executionMode)
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
    [MemberData(nameof(ExecutionModes))]
    public void TestLessOrEqualThan1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 <= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestLessOrEqualThan2(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 ≤ var2", variables);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestGreaterThan1(ExecutionMode executionMode)
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
    [MemberData(nameof(ExecutionModes))]
    public void TestGreaterOrEqualThan1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 >= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestNotEqual1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 != 2", variables);
        Assert.Equal(0.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestNotEqual2(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "var2", 2 }
        };

        var result = engine.Calculate("var1 ≠ 2", variables);
        Assert.Equal(0.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestEqual(ExecutionMode executionMode)
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

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestVariableUnderscore(ExecutionMode executionMode)
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

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCustomFunctionFunc11(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        engine.AddFunction("test", CustomFunction);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        const double expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
        return;

        double CustomFunction(double a, double b, double c, double d, double e, double f, double g, double h, double i, double j, double k) 
            => a + b + c + d + e + f + g + h + i + j + k;
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCustomFunctionDynamicFunc(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
        return;

        double DoSomething(params double[] a)
        {
            return a.Sum();
        }
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCustomFunctionDynamicFuncNested(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = false,
            OptimizerEnabled = false,
            CaseSensitive = true
        });
        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,test(4,5,6)) + test(7,8,9,10,11)");

        const double expected = (11 * (11 + 1)) / 2.0;

        Assert.Equal(expected, result);
        return;

        static double DoSomething(params double[] a)
        {
            return a.Sum();
        }
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestAnd(ExecutionMode executionMode)
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
    [MemberData(nameof(ExecutionModes))]
    public void TestOr1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("(1 || 0)");
        Assert.Equal(1, result);
    }


    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestOr2(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("(0 || 0)");
        Assert.Equal(0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestMedian1Compiled(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("median(3,1,5,4,2)");
        Assert.Equal(3, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestMedian2Compiled(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("median(3,1,5,4)");
        Assert.Equal(3, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationFormulaBuildingWithConstants1(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationFormulaBuildingWithConstants2(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);

        var formula = (Func<double, double, double>)engine.Formula("a+b+c")
            .Parameter("b", DataType.FloatingPoint)
            .Parameter("c", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0, 2.0);
        Assert.Equal(5.0, result);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationFormulaBuildingWithConstants3(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = true
        });

        var formula = (Func<double, double>)engine.Formula("a+A")
            .Parameter("A", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache1()
    {
        var engine = new CalculationEngine(new JaceOptions { CacheEnabled = true });

        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);

        Assert.Throws<VariableNotDefinedException>(() =>
        {
            engine.Build("a+b+c")
                (new Dictionary<string, double> { { "b", 3 }, { "c", 3 } });
        });
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache2()
    {
        var engine = new CalculationEngine(new JaceOptions { CacheEnabled = true });
        var fn = engine.Build("a+b+c");
        var result = fn(new Dictionary<string, double> { { "a", 1 }, { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);


        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(6.0, result1);
    }


    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationFormulaBuildingWithConstantsCache3(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = executionMode,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = true
        });
        var formula = (Func<double, double>)engine.Formula("a+A")
            .Parameter("A", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0);
        Assert.Equal(3.0, result);

        var formula1 = (Func<double, double, double>)engine.Formula("a+A")
            .Parameter("A", DataType.FloatingPoint)
            .Parameter("a", DataType.FloatingPoint)
            .Result(DataType.FloatingPoint)
            .Build();

        var result1 = formula1(2.0, 2.0);
        Assert.Equal(4.0, result1);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationFormulaBuildingWithConstantsCache4(ExecutionMode executionMode)
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);

        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(6.0, result1);
    }
    
    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache5()
    {
        var engine = new CalculationEngine(new JaceOptions { CacheEnabled = true });

        var fn = engine.Build("a+b+c");
        var result = fn(new Dictionary<string, double> { { "a", 1 }, { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);


        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 3 }, { "c", 3 } });
        Assert.Equal(8.0, result1);
    }

    [Theory]
    [MemberData(nameof(ExecutionModes))]
    public void TestCalculationCompiledExpression(ExecutionMode executionMode)
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