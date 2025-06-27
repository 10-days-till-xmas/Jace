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
    public abstract class TestExecutionModeBase(ExecutionMode mode)
    {
        protected readonly ExecutionMode executionMode = mode;
        
        [Theory]
        [InlineData("2+3", 5.0)]
        [InlineData("2.0+3.0", 5.0)]
        public void TestCalculateFormula1(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions{ExecutionMode = executionMode});
            var result = engine.Calculate(formula);

            Assert.Equal(expected, result);
        }
        
        #region Operator Tests
        [Fact]
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
        public void TestCalculatePow()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("2^3.0");

            Assert.Equal(8.0, result);
        }
        #endregion
        
        #region Function Tests
        [Fact]
        public void TestCalculateSineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("sin(14)");

            Assert.Equal(Math.Sin(14.0), result);
        }

        [Fact]
        public void TestCalculateCosineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("cos(41)");

            Assert.Equal(Math.Cos(41.0), result);
        }

        [Fact]
        public void TestCalculateLognFunction()
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
        #endregion
        
        #region VariableTests
        [Fact]
        public void TestCalculateFormulaWithVariables()
        {
            var variables = new Dictionary<string, double>
            {
                { "var1", 2.5 },
                { "var2", 3.4 }
            };
            var engine = new CalculationEngine(new JaceOptions{ExecutionMode = executionMode});
            var result = engine.Calculate("var1*var2", variables);

            Assert.Equal(8.5, result);
        }

        [Fact]
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
        public void TestCalculateFormulaVariableNotDefined()
        {
            var variables = new Dictionary<string, double> { { "var1", 2.5 } };

            Assert.Throws<VariableNotDefinedException>(() =>
            {
                new CalculationEngine(CultureInfo.InvariantCulture, executionMode)
                    .Calculate("var1*var2", variables);
            });
        }
        #endregion
        
        [Theory]
        [InlineData("-(1+2+(3+4))", -10.0)]
        [InlineData("5+(-(1*2))", 3.0)]
        [InlineData("5*(-(1*2)*3)",-30.0)]
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
        
        [Fact]
        public void TestBuild()
        {
            var engine = new CalculationEngine(new JaceOptions{ExecutionMode = executionMode});
            Func<Dictionary<string, double>, double> function = engine.Build("var1+2*(3*age)");

            var variables = new Dictionary<string, double>
            {
                { "var1", 2 },
                { "age", 4 }
            };

            var result = function(variables);
            Assert.Equal(26.0, result);
        }

        [Fact]
        public void TestFormulaBuilder()
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

        [Fact]
        public void TestFormulaBuilderConstant()
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

        [Fact]
        public void TestVariableNameCaseSensitivity()
        {
            var variables = new Dictionary<string, double> { { "testvariable", 42.5 } };

            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("2 * tEsTVaRiaBlE", variables);

            Assert.Equal(85.0, result);
        }

        [Fact]
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
        public void TestCustomFunction()
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

        [Fact]
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
            Assert.Equal(2+2/(3*4.2), result);
        }

        [Fact]
        public void TestVariableCaseNonFunc()
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

        [Fact]
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
        [InlineData("var1 <= var2", 1.0)]
        [InlineData("var1 ≤ var2", 1.0)]
        public void TestLessOrEqualThan1(string formula,  double expected)
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
        [InlineData("var1 >= var2", 1.0)]
        [InlineData("var1 ≥ var2", 1.0)]
        public void TestGreaterOrEqualThan1(string formula,  double expected)
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
        [InlineData("var1 != 2", 0.0)]
        [InlineData("var1 ≠ 2", 0.0)]
        public void TestNotEqual(string formula,  double expected)
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

        [Fact]
        public void TestCustomFunctionFunc11()
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

        [Fact]
        public void TestCustomFunctionDynamicFunc()
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

        [Fact]
        public void TestCustomFunctionDynamicFuncNested()
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

        [Fact]
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
        [InlineData("(1 || 0)", 1)]
        [InlineData("(0 || 0)", 0)]
        public void TestOr1(string  formula, double expected)
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
        [InlineData("median(3,1,5,4,2)", 3)]
        [InlineData("median(3,1,5,4)",3)]
        public void TestMedian(string formula, double expected)
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
        public void TestCalculationFormulaBuildingWithConstants1()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
            var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
            Assert.Equal(5.0, result);
        }

        [Fact]
        public void TestCalculationFormulaBuildingWithConstants2()
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

        [Fact]
        public void TestCalculationFormulaBuildingWithConstants3()
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
        
        [Fact]
        public void TestCalculationFormulaBuildingWithConstantsCache3()
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

        [Fact]
        public void TestCalculationFormulaBuildingWithConstantsCache4()
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

        [Fact]
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

    public class TestCalculationEngine_Interpreted() : TestExecutionModeBase(ExecutionMode.Interpreted);
    public class TestCalculationEngine_Compiled() : TestExecutionModeBase(ExecutionMode.Compiled);
}