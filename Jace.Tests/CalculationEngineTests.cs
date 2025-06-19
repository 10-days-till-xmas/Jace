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
    [Fact]
    public void TestCalculationFormula1FloatingPointCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var result = engine.Calculate("2.0+3.0");

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationFormula1IntegersCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var result = engine.Calculate("2+3");

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculateFormula1()
    {
        var engine = new CalculationEngine();
        var result = engine.Calculate("2+3");

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculateModuloCompiled()
    {
        CalculationEngine engine =
            new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, true);
        double result = engine.Calculate("5 % 3.0");

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestCalculateModuloInterpreted()
    {
        CalculationEngine engine =
            new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, true);
        double result = engine.Calculate("5 % 3.0");

        Assert.Equal(2.0, result);
    }

    [Fact]
    public void TestCalculatePowCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var result = engine.Calculate("2^3.0");

        Assert.Equal(8.0, result);
    }

    [Fact]
    public void TestCalculatePowInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
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

    [Fact]
    public void TestCalculateFormulaWithCaseSensitiveVariables1Compiled()
    {
        var variables = new Dictionary<string, double>
        {
            { "VaR1", 2.5 },
            { "vAr2", 3.4 },
            { "var1", 1 },
            { "var2", 1 }
        };

            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, false);
            double result = engine.Calculate("VaR1*vAr2", variables);

        Assert.Equal(8.5, result);
    }

    [Fact]
    public void TestCalculateFormulaWithCaseSensitiveVariables1Interpreted()
    {
        var variables = new Dictionary<string, double>
        {
            { "VaR1", 2.5 },
            { "vAr2", 3.4 },
            { "var1", 1 },
            { "var2", 1 }
        };

        CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, false);
        var result = engine.Calculate("VaR1*vAr2", variables);

        Assert.Equal(8.5, result);
    }

    [Fact]
    public void TestCalculateFormulaVariableNotDefinedInterpreted()
    {
        var variables = new Dictionary<string, double> { { "var1", 2.5 } };

        Assert.Throws<VariableNotDefinedException>(() =>
        {
            new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted)
                .Calculate("var1*var2", variables);
        });
    }

    [Fact]
    public void TestCalculateFormulaVariableNotDefinedCompiled()
    {
        var variables = new Dictionary<string, double> { { "var1", 2.5 } };

        Assert.Throws<VariableNotDefinedException>(() =>
        {
            new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled)
                .Calculate("var1*var2", variables);
        });
    }

    [Fact]
    public void TestCalculateSineFunctionInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        var result = engine.Calculate("sin(14)");

        Assert.Equal(Math.Sin(14.0), result);
    }

    [Fact]
    public void TestCalculateSineFunctionCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("sin(14)");

        Assert.Equal(Math.Sin(14.0), result);
    }

    [Fact]
    public void TestCalculateCosineFunctionInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        var result = engine.Calculate("cos(41)");

        Assert.Equal(Math.Cos(41.0), result);
    }

    [Fact]
    public void TestCalculateCosineFunctionCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("cos(41)");

        Assert.Equal(Math.Cos(41.0), result);
    }

    [Fact]
    public void TestCalculateLognFunctionInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("logn(14, 3)");

        Assert.Equal(Math.Log(14.0, 3.0), result);
    }

    [Fact]
    public void TestCalculateLognFunctionCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("logn(14, 3)");

        Assert.Equal(Math.Log(14.0, 3.0), result);
    }

    [Fact]
    public void TestNegativeConstant()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("-100");

        Assert.Equal(-100.0, result);
    }

    [Fact]
    public void TestMultiplicationWithNegativeConstant()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("5*-100");

        Assert.Equal(-500.0, result);
    }

    [Fact]
    public void TestUnaryMinus1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("-(1+2+(3+4))");

        Assert.Equal(-10.0, result);
    }

    [Fact]
    public void TestUnaryMinus1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("-(1+2+(3+4))");

        Assert.Equal(-10.0, result);
    }

    [Fact]
    public void TestUnaryMinus2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("5+(-(1*2))");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestUnaryMinus2Interpreted()
    {

        var engine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Interpreted,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5+(-(1*2))");

        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestUnaryMinus3Compiled()
    {
        

        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
                                           ExecutionMode.Compiled,
                                           true,
                                           false,
                                           true);
        var eng = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Compiled,
            CacheEnabled = true,
            OptimizerEnabled = false,
            CaseSensitive = false
        });
        var result = engine.Calculate("5*(-(1*2)*3)");

        Assert.Equal(-30.0, result);
    }

    [Fact]
    public void TestUnaryMinus3Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("5*(-(1*2)*3)");

        Assert.Equal(-30.0, result);
    }

    [Fact]
    public void TestUnaryMinus4Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("5* -(1*2)");

        Assert.Equal(-10.0, result);
    }

    [Fact]
    public void TestUnaryMinus4Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("5* -(1*2)");

        Assert.Equal(-10.0, result);
    }

    [Fact]
    public void TestUnaryMinus5Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("-(1*2)^3");

        Assert.Equal(-8.0, result);
    }

    [Fact]
    public void TestUnaryMinus5Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("-(1*2)^3");

        Assert.Equal(-8.0, result);
    }

    [Fact]
    public void TestBuild()
    {
        var engine = new CalculationEngine();
        Func<Dictionary<string, double>, double> function = engine.Build("var1+2*(3*age)");

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("age", 4);

        var result = function(variables);
        Assert.Equal(26.0, result);
    }

    [Fact]
    public void TestFormulaBuilder()
    {
        var engine = new CalculationEngine();
        var function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
            .Parameter("var1", DataType.Integer)
            .Parameter("age", DataType.FloatingPoint)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = function(2, 4);
        Assert.Equal(26.0, result);
    }

    [Fact]
    public void TestFormulaBuilderCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var function = (Func<int, double, double>)engine.Formula("var1+2*(3*age)")
            .Parameter("var1", DataType.Integer)
            .Parameter("age", DataType.FloatingPoint)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = function(2, 4);
        Assert.Equal(26.0, result);
    }

    [Fact]
    public void TestFormulaBuilderConstantInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        engine.AddConstant("age", 18.0);

        var function = (Func<int, double>)engine.Formula("age+var1")
            .Parameter("var1", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = function(3);
        Assert.Equal(21.0, result);
    }

    [Fact]
    public void TestFormulaBuilderConstantCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
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
            var function = (Func<int, double, double>)engine.Formula("sin+2")
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
            var function = (Func<int, double, double>)engine.Formula("var1+2")
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
            var variables = new Dictionary<string, double>();
            variables.Add("pi", 2.0);

            var engine = new CalculationEngine();
            var result = engine.Calculate("2 * pI", variables);
        });
    }

    [Fact]
    public void TestVariableNameCaseSensitivityCompiled()
    {
        var variables = new Dictionary<string, double>();
        variables.Add("blabla", 42.5);

        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var result = engine.Calculate("2 * BlAbLa", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    public void TestVariableNameCaseSensitivityInterpreted()
    {
        var variables = new Dictionary<string, double>();
        variables.Add("blabla", 42.5);

        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        var result = engine.Calculate("2 * BlAbLa", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    public void TestVariableNameCaseSensitivityNoToLowerCompiled()
    {
        var variables = new Dictionary<string, double>();
        variables.Add("BlAbLa", 42.5);

        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, false, false, false);
        var result = engine.Calculate("2 * BlAbLa", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    public void TestVariableNameCaseSensitivityNoToLowerInterpreted()
    {
        var variables = new Dictionary<string, double>();
        variables.Add("BlAbLa", 42.5);

            CalculationEngine engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, false, false, false);
            double result = engine.Calculate("2 * BlAbLa", variables);

        Assert.Equal(85.0, result);
    }

    [Fact]
    public void TestCustomFunctionInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Interpreted, false, false, false);
        engine.AddFunction("test", (a, b) => a + b);

        var result = engine.Calculate("test(2,3)");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCustomFunctionCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Compiled, false, false, false);
        engine.AddFunction("test", (a, b) => a + b);

        var result = engine.Calculate("test(2,3)");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestComplicatedPrecedence1()
    {
        var engine = new CalculationEngine();

        var result = engine.Calculate("1+2-3*4/5+6-7*8/9+0");
        Assert.Equal(0.378, Math.Round(result, 3));
    }

    [Fact]
    public void TestComplicatedPrecedence2()
    {
        var engine = new CalculationEngine();

        var result = engine.Calculate("1+2-3*4/sqrt(25)+6-7*8/9+0");
        Assert.Equal(0.378, Math.Round(result, 3));
    }

    [Fact]
    public void TestExpressionArguments1()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate("ifless(0.57, (3000-500)/(1500-500), 10, 20)");
        Assert.Equal(10, result);
    }

    [Fact]
    public void TestExpressionArguments2()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate("if(0.57 < (3000-500)/(1500-500), 10, 20)");
        Assert.Equal(10, result);
    }

    [Fact]
    public void TestNestedFunctions()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture);

        var result = engine.Calculate("max(sin(67), cos(67))");
        Assert.Equal(-0.517769799789505, Math.Round(result, 15));
    }

    [Fact]
    public void TestVariableCaseFuncInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("otherVariable", 4.2);

        var result = formula(variables);
    }

    [Fact]
    public void TestVariableCaseFuncCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        Func<Dictionary<string, double>, double> formula = engine.Build("var1+2/(3*otherVariablE)");

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("otherVariable", 4.2);

        var result = formula(variables);
    }

    [Fact]
    public void TestVariableCaseNonFunc()
    {
        var engine = new CalculationEngine();

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("otherVariable", 4.2);

        var result = engine.Calculate("var1+2/(3*otherVariablE)", variables);
    }

    [Fact]
    public void TestLessThanInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 4.2);

        var result = engine.Calculate("var1 < var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestLessThanCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 4.2);

        var result = engine.Calculate("var1 < var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestLessOrEqualThan1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 <= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestLessOrEqualThan1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 <= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestLessOrEqualThan2Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 ≤ var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestLessOrEqualThan2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 ≤ var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestGreaterThan1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 3);

        var result = engine.Calculate("var1 > var2", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestGreaterThan1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 3);

        var result = engine.Calculate("var1 > var2", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestGreaterOrEqualThan1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 >= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestGreaterOrEqualThan1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 >= var2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestNotEqual1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 != 2", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestNotEqual2Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 ≠ 2", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestNotEqual2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 ≠ 2", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestEqualInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 == 2", variables);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void TestEqualCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var1", 2);
        variables.Add("var2", 2);

        var result = engine.Calculate("var1 == 2", variables);
        Assert.Equal(1.0, result);
    }

    public void TestVariableUnderscoreInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var variables = new Dictionary<string, double>();
        variables.Add("var_var_1", 1);
        variables.Add("var_var_2", 2);

        var result = engine.Calculate("var_var_1 + var_var_2", variables);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestVariableUnderscoreCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var variables = new Dictionary<string, double>();
        variables.Add("var_var_1", 1);
        variables.Add("var_var_2", 2);

        var result = engine.Calculate("var_var_1 + var_var_2", variables);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestCustomFunctionFunc11Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Interpreted, false, false, false);
        engine.AddFunction("test", (a, b, c, d, e, f, g, h, i, j, k) => a + b + c + d + e + f + g + h + i + j + k);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCustomFunctionFunc11Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Compiled, false, false, false);
        engine.AddFunction("test", (a, b, c, d, e, f, g, h, i, j, k) => a + b + c + d + e + f + g + h + i + j + k);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCustomFunctionDynamicFuncInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Interpreted, false, false, false);

        double DoSomething(params double[] a)
        {
            return a.Sum();
        }

        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCustomFunctionDynamicFuncCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Compiled, false, false, false);

        double DoSomething(params double[] a)
        {
            return a.Sum();
        }

        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,4,5,6,7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCustomFunctionDynamicFuncNestedInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Interpreted, false, false, false);

        double DoSomething(params double[] a)
        {
            return a.Sum();
        }

        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,test(4,5,6)) + test(7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCustomFunctionDynamicFuncNestedDynamicCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture,
            ExecutionMode.Compiled, false, false, false);

        double DoSomething(params double[] a)
        {
            return a.Sum();
        }

        engine.AddFunction("test", DoSomething);
        var result = engine.Calculate("test(1,2,3,test(4,5,6)) + test(7,8,9,10,11)");
        var expected = (11 * (11 + 1)) / 2.0;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestAndCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("(1 && 0)");
        Assert.Equal(0, result);
    }

    [Fact]
    public void TestAndInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("(1 && 0)");
        Assert.Equal(0, result);
    }

    [Fact]
    public void TestOr1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("(1 || 0)");
        Assert.Equal(1, result);
    }

    [Fact]
    public void TestOr1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("(1 || 0)");
        Assert.Equal(1, result);
    }

    [Fact]
    public void TestOr2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("(0 || 0)");
        Assert.Equal(0, result);
    }

    [Fact]
    public void TestOr2Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("(0 || 0)");
        Assert.Equal(0, result);
    }

    [Fact]
    public void TestMedian1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("median(3,1,5,4,2)");
        Assert.Equal(3, result);
    }

    [Fact]
    public void TestMedian1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("median(3,1,5,4,2)");
        Assert.Equal(3, result);
    }

    [Fact]
    public void TestMedian2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, false, true);
        var result = engine.Calculate("median(3,1,5,4)");
        Assert.Equal(3, result);
    }

    [Fact]
    public void TestMedian2Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, false, true);
        var result = engine.Calculate("median(3,1,5,4)");
        Assert.Equal(3, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants1Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants1Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants2Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);

        var formula = (Func<double, double, double>)engine.Formula("a+b+c")
            .Parameter("b", DataType.FloatingPoint)
            .Parameter("c", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0, 2.0);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants2Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);

        var formula = (Func<double, double, double>)engine.Formula("a+b+c")
            .Parameter("b", DataType.FloatingPoint)
            .Parameter("c", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0, 2.0);
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants3Compiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, false);

        var formula = (Func<double, double>)engine.Formula("a+A")
            .Parameter("A", DataType.FloatingPoint)
            .Constant("a", 1)
            .Result(DataType.FloatingPoint)
            .Build();

        var result = formula(2.0);
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstants3Interpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, false);

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
            var fn1 = engine.Build("a+b+c");
            var result1 = fn1(new Dictionary<string, double> { { "b", 3 }, { "c", 3 } });
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


    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache3()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, false);

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


    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache4()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, false);

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

    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache5()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);

        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(6.0, result1);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache6()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted);
        var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
        var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);

        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
        Assert.Equal(6.0, result1);
    }

    [Fact]
    public void TestCalculationFormulaBuildingWithConstantsCache7()
    {
        var engine = new CalculationEngine(new JaceOptions { CacheEnabled = true });

        var fn = engine.Build("a+b+c");
        var result = fn(new Dictionary<string, double> { { "a", 1 }, { "b", 2 }, { "c", 2 } });
        Assert.Equal(5.0, result);


        var fn1 = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 2 } });
        var result1 = fn1(new Dictionary<string, double> { { "b", 3 }, { "c", 3 } });
        Assert.Equal(8.0, result1);
    }

    [Fact]
    public void TestCalculationCompiledExpressionCompiled()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Compiled, true, true, false);

        Expression<Func<double, double, double>> expression = (a, b) => a + b;
        expression.Compile();

        engine.AddFunction("test", expression.Compile());

        var result = engine.Calculate("test(2, 3)");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void TestCalculationCompiledExpressionInterpreted()
    {
        var engine = new CalculationEngine(CultureInfo.InvariantCulture, ExecutionMode.Interpreted, true, true, false);

        Expression<Func<double, double, double>> expression = (a, b) => a + b;
        expression.Compile();

        engine.AddFunction("test", expression.Compile());

        var result = engine.Calculate("test(2, 3)");
        Assert.Equal(5.0, result);
    }
}