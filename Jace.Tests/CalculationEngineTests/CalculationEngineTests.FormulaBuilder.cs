using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Jace.Tests;

public partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        [Fact]
        [Trait("Category", "FormulaBuilder")]
        public void TestBuild()
        {
            var engine = new CalculationEngine(new JaceOptions { ExecutionMode = executionMode });
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
        public void TestCalculationFormulaBuildingWithConstants1()
        {
            var engine = new CalculationEngine(CultureInfo.InvariantCulture, executionMode);
            var fn = engine.Build("a+b+c", new Dictionary<string, double> { { "a", 1 } });
            var result = fn(new Dictionary<string, double> { { "b", 2 }, { "c", 2 } });
            Assert.Equal(5.0, result);
        }

        [Fact]
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
        [Trait("Category", "FormulaBuilder")]
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
    }
}