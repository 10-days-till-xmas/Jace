using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Jace.Tests;

public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        [Fact]
        [Trait("Category", "Custom Functions")]
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
            Delegate customFunction = (Func<double, double, double>)((a, b) => a + b);
            engine.AddFunction("test", customFunction);

            var result = engine.Calculate("test(2,3)");
            Assert.Equal(5.0, result);
        }

        [Fact]
        [Trait("Category", "Custom Functions")]
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

            double CustomFunction(double a, double b, double c, double d, double e, double f, double g, double h,
                double i, double j, double k)
                => a + b + c + d + e + f + g + h + i + j + k;
        }

        [Fact]
        [Trait("Category", "Custom Functions")]
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
        [Trait("Category", "Custom Functions")]
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
    }
}