using Xunit;

namespace Jace.Tests;

public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        [Theory]
        [Trait("Category", "PEMDAS/BODMAS")]
        [InlineData("2+3*4", 14)]
        [InlineData("2+3*4-5", 9)]
        public void TestCalculateMultBeforeAddSub(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
            var result = engine.Calculate(formula);

            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Category", "PEMDAS/BODMAS")]
        [InlineData("2+6/2", 5)]
        [InlineData("2+3/4-5", -2.25)]
        public void TestCalculateDivBeforeAddSub(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
            var result = engine.Calculate(formula);

            Assert.Equal(expected, result);
        }

        [Theory]
        [Trait("Category", "PEMDAS/BODMAS")]
        [InlineData("4*3^2", 36)]
        [InlineData("3*4^2*2", 96)]
        public void TestCalculatePowBeforeMultDiv(string formula, double expected)
        {
            var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
            var result = engine.Calculate(formula);

            Assert.Equal(expected, result);
        }
    }
}