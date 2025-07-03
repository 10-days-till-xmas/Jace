using System;
using System.Globalization;
using Xunit;

namespace Jace.Tests;

public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        [Fact]
        [Trait("Category", "Functions")]
        public void TestCalculateSineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("sin(14)");

            Assert.Equal(Math.Sin(14.0), result);
        }

        [Fact]
        [Trait("Category", "Functions")]
        public void TestCalculateCosineFunction()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var result = engine.Calculate("cos(41)");

            Assert.Equal(Math.Cos(41.0), result);
        }

        [Fact]
        [Trait("Category", "Functions")]
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

        [Theory]
        [Trait("Category", "Functions")]
        [InlineData("median(3,1,5,4,2)", 3)]
        [InlineData("median(3,1,5,4)", 3)]
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
        [Trait("Category", "Functions")]
        public void TestNestedFunctions()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture);

            var result = engine.Calculate("max(sin(67), cos(67))");
            Assert.Equal(-0.517769799789505, Math.Round(result, 15));
        }
    }
}