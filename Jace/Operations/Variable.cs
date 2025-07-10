using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Execution;

namespace Jace.Operations;

/// <summary>
/// Represents a variable in a mathematical formula.
/// </summary>
public sealed class Variable(string name) : Operation(DataType.FloatingPoint, true, false)
{
    public string Name { get; } = name;

    public override bool Equals(object? obj)
    {
        if (obj is Variable other)
        {
            return Name.Equals(other.Name);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        return variables.TryGetValue(Name, out var value)
            ? value
            : throw new VariableNotDefinedException(
                $"The variable \"{Name}\" used is not defined.");
    }

    public override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        var getVariableValueOrThrow = DynamicCompiler.PrecompiledMethods.GetVariableValueOrThrow;
        return Expression.Call(null,
            getVariableValueOrThrow.GetMethodInfo(),
            Expression.Constant(Name),
            contextParameter);
    }
}