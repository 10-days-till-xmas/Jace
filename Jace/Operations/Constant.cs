using System.Collections.Generic;
using System.Linq.Expressions;
using Jace.Execution;

namespace Jace.Operations;

public abstract class Constant<T>(DataType dataType, T value) : Operation(dataType, false, true) where T : notnull
{
    public T Value { get; } = value;

    public override bool Equals(object? obj)
    {
        if (obj is Constant<T> other)
            return Value.Equals(other.Value);
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public sealed class IntegerConstant(int value) : Constant<int>(DataType.Integer, value)
{
    public override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        return Value;
    }

    public override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        return Expression.Constant((double)Value, typeof(double));
    }
}

public sealed class FloatingPointConstant(double value) : Constant<double>(DataType.FloatingPoint, value)
{
    public override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        return Value;
    }

    public override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        return Expression.Constant(Value, typeof(double));
    }
}