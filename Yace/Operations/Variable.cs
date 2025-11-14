using System.Linq.Expressions;
using System.Reflection;

namespace Yace.Operations;

/// <summary>
/// Represents a variable in a mathematical formula.
/// </summary>
public sealed class Variable(string name) : Operation(DataType.FloatingPoint, true, false)
{
    public string Name { get; } = name;

    public override bool Equals(object? obj) => obj is Variable other
                                             && Name.Equals(other.Name);

    public override int GetHashCode() => Name.GetHashCode();

    public override double Execute(FormulaContext context)
    {
        if (context.Variables == null)
            throw new VariableNotDefinedException("No variables were provided.");
        if (!context.Variables.TryGetValue(Name, out var varValue))
            throw new VariableNotDefinedException($"The variable \"{Name}\" used is not defined.");
        return varValue;
    }

    public override Expression ExecuteDynamic(ParameterExpression contextParameter)
    {
        var getVariableValueOrThrow = GetVariableValueOrThrow;
        return Expression.Call(null,
            getVariableValueOrThrow.GetMethodInfo(),
            Expression.Constant(Name),
            contextParameter);
    }
    
    private static double GetVariableValueOrThrow(string variableName, FormulaContext context)
    {
        if (context.Variables!.TryGetValue(variableName, out var variable))
            return variable;
        if (context.ConstantRegistry!.TryGetConstantInfo(variableName, out var info))
            return info.Value;
        throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
    }
}
