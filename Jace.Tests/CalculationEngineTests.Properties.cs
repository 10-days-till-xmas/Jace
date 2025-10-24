using System;
using FsCheck.Xunit;
using Jace.Tests.Helpers;
using Jace.Util;
using Xunit;

namespace Jace.Tests;
public static partial class CalculationEngineTests
{
    public abstract partial class TestExecutionModeBase
    {
        private CalculationEngine Engine => new(new JaceOptions { ExecutionMode = executionMode });

        [Property]
        public void Evaluate_Addition_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} + {b}");
            Assert.Equal(a + b, result);
        }
        [Property]
        public void Evaluate_Addition_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a.ToString().Replace("+", "")} + {b.ToString().Replace("+", "")}");
            Assert.Equal(a + b, result);
        }

        [Property]
        public void Evaluate_Subtraction_Int(int a, int b)
        {
            
            var result = Engine.Calculate($"{a} - {b}");
            Assert.Equal(a - b, result);
        }
        [Property]
        public void Evaluate_Subtraction_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} - {b}");
            Assert.Equal(a - b, result);
        }

        [Property]
        public void Evaluate_Multiplication_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} * {b}");
            Assert.Equal(a * b, result);
        }
        [Property]
        public void Evaluate_Multiplication_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} * {b}");
            Assert.Equal(a * b, result);
        }

        [Property]
        public void Evaluate_Division_Int(int a, int b)
        {
            if (b == 0) return;
            var result = Engine.Calculate($"{a} / {b}");
            Assert.Equal((double)a / b, result);
        }

        [Property]
        public void Evaluate_Division_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            if (b == 0) return;
            var result = Engine.Calculate($"{a} / {b}");
            Assert.Equal(a / b, result);
        }

        [Property]
        public void Evaluate_Modulo_Int(int a, int b)
        {
            if (b == 0) return;
            var result = Engine.Calculate($"{a} % {b}");
            Assert.Equal(a % b, result);
        }
        [Property]
        public void Evaluate_Modulo_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            if (b == 0) return;
            var result = Engine.Calculate($"{a} % {b}");
            Assert.Equal(a % b, result);
        }

        [Property]
        public void Evaluate_Pow_Int(int a, int b)
        {
            if (b == 0) return;
            var result = Engine.Calculate($"{a} ^ {b}");
            Assert.Equal(Math.Pow(a, b), result);
        }

        [Property]
        public void Evaluate_Pow_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            if (b == 0) return;
            var result = Engine.Calculate($"{a} ^ {b}");
            Assert.Equal(Math.Pow(a, b), result);
        }

        [Property]
        public void Evaluate_LessThan_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} < {b}");
            Assert.Equal((a < b).AsDouble(), result);
        }
        
        [Property]
        public void Evaluate_LessThan_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} < {b}");
            Assert.Equal((a < b).AsDouble(), result);
        }

        [Property]
        public void Evaluate_LessThanOrEqual_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} <= {b}");
            var result2 = Engine.Calculate($"{a} ≤ {b}");
            Assert.Equal((a <= b).AsDouble(), result);
            Assert.Equal((a <= b).AsDouble(), result2);
        }
        
        [Property]
        public void Evaluate_LessThanOrEqual_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} <= {b}");
            var result2 = Engine.Calculate($"{a} ≤ {b}");
            Assert.Equal((a <= b).AsDouble(), result);
            Assert.Equal((a <= b).AsDouble(), result2);
        }

        [Property]
        public void Evaluate_GreaterThan_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} > {b}");
            Assert.Equal((a > b).AsDouble(), result);
        }
        [Property]
        public void Evaluate_GreaterThan_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} > {b}");
            Assert.Equal((a > b).AsDouble(), result);
        }

        [Property]
        public void Evaluate_GreaterThanOrEqual_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} >= {b}");
            var result2 = Engine.Calculate($"{a} ≥ {b}");
            Assert.Equal((a >= b).AsDouble(), result);
            Assert.Equal((a >= b).AsDouble(), result2);
        }
        
        [Property]
        public void Evaluate_GreaterThanOrEqual_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} >= {b}");
            var result2 = Engine.Calculate($"{a} ≥ {b}");
            Assert.Equal((a >= b).AsDouble(), result);
            Assert.Equal((a >= b).AsDouble(), result2);
        }

        [Property]
        public void Evaluate_NotEqual_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} != {b}");
            var result2 = Engine.Calculate($"{a} ≠ {b}");
            Assert.Equal((a != b).AsDouble(), result);
            Assert.Equal((a != b).AsDouble(), result2);
        }
        
        [Property]
        public void Evaluate_NotEqual_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} != {b}");
            var result2 = Engine.Calculate($"{a} ≠ {b}");
            Assert.Equal((!a.Equals(b)).AsDouble(), result);
            Assert.Equal((!a.Equals(b)).AsDouble(), result2);
        }

        [Property]
        public void Evaluate_Equal_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} == {b}");
            Assert.Equal((a == b).AsDouble(), result);
        }
        [Property]
        public void Evaluate_Equal_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} == {b}");
            Assert.Equal((a.Equals(b)).AsDouble(), result);
        }

        [Property]
        public void Evaluate_And_Int(int a, int b)
        {
            var result = Engine.Calculate($"{a} && {b}");
            Assert.Equal(((a != 0) && (b != 0)).AsDouble(), result);
        }
        
        [Property]
        public void Evaluate_And_Double(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"{a} && {b}");
            Assert.Equal((a.AsBool() && b.AsBool()).AsDouble(), result);
        }

        [Property]
        public void Evaluate_Or(int a, int b)
        {
            var result = Engine.Calculate($"{a} || {b}");
            Assert.Equal(((a != 0) || (b != 0)).AsDouble(), result);
        }

        [Property]
        public void Evaluate_UnaryMinus_Int(uint a)
        {
            var result = Engine.Calculate($"-{a}");
            Assert.Equal(-a, result);
        }
        
        [Property]
        public void Evaluate_UnaryMinus_Double(double a)
        {
            if (a.IsInvalid()) return;
            a = Math.Abs(a);
            var result = Engine.Calculate($"-{a}");
            Assert.Equal(-a, result);
        }

        [Property]
        public void Evaluate_SineFunction(double a)
        {
            if (a.IsInvalid()) return;
            var result = Engine.Calculate($"sin({a})");
            Assert.Equal(Math.Sin(a), result);
        }
        [Property]
        public void Evaluate_CosineFunction(double a)
        {
            if (a.IsInvalid()) return;
            var result = Engine.Calculate($"cos({a})");
            Assert.Equal(Math.Cos(a), result);
        }
        [Property]
        public void Evaluate_LognFunction(double a, double b)
        {
            if (a.IsInvalid() || b.IsInvalid()) return;
            var result = Engine.Calculate($"logn({a}, {b})");
            Assert.Equal(Math.Log(a, b), result);
        }
    }
}