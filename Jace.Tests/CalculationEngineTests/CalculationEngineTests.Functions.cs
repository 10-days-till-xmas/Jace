using System;
using System.Globalization;
using Xunit;

namespace Jace.Tests;

public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        public static TheoryData<string> FormulaTestData => new()
        {
            "sin(1)", "cos(1)", "csc(1)", "sec(1)", "asin(0.1)", "acos(0.1)", "tan(1)", "cot(1)",
            "atan(0.1)", "acot(0.1)", "loge(1)", "log10(1)", "logn(2,2)", "sqrt(1)", "abs(1)",
            "if(1,2,3)", "ifless(1,2,3,4)", "ifmore(1,2,3,4)", "ifequal(1,2,3,4)",
            "ceiling(1)", "floor(1)", "truncate(1)", "round(1)",
            "max(1,2,3)", "min(1,2,3)", "avg(1,2,3)", "median(1,2,3)", "random()"
        };
        
        [Theory]
        [Trait("Category", "Functions")]
        [MemberData(nameof(FormulaTestData))]
        public void TestCalculate_Functions(string formula)
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            engine.Calculate(formula);
        }
        
        [Fact]
        [Trait("Category", "Functions")]
        public void TestCalculate_SineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("sin(14)");

            Assert.Equal(Math.Sin(14.0), result);
        }

        [Fact]
        [Trait("Category", "Functions")]
        public void TestCalculate_CosineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("cos(41)");

            Assert.Equal(Math.Cos(41.0), result);
        }

        [Fact]
        [Trait("Category", "Functions")]
        public void TestCalculate_LognFunction()
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

        [Theory]
        [Trait("Category", "Functions")]
        [InlineData("median(3,1,5,4,2)", 3)]
        [InlineData("median(3,1,5,4)", 3)]
        public void TestCalculate_MedianFunction(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions
            {
                CultureInfo = CultureInfo.InvariantCulture,
                ExecutionMode = executionMode,
                CacheEnabled = true,
                OptimizerEnabled = true,
                CaseSensitive = false
            });
            var result = engine.Calculate(formula);
            Assert.Equal(expected, result);
        }
        [Theory]
        [Trait("Category", "Functions")]
        [InlineData("max(3,1,5,4,2)", 5)]
        public void TestCalculate_MaxFunction(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions
            {
                CultureInfo = CultureInfo.InvariantCulture,
                ExecutionMode = executionMode,
                CacheEnabled = true,
                OptimizerEnabled = true,
                CaseSensitive = false
            });
            var result = engine.Calculate(formula);
            Assert.Equal(expected, result);
        }


        [Fact]
        [Trait("Category", "Functions")]
        public void TestNestedFunctions()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture);

            var result = engine.Calculate("max(sin(67), cos(67))");
            Assert.Equal(-0.517769799789505, Math.Round(result, 15));
        }
    }
}