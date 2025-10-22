using System;
using System.Linq.Expressions;

namespace Jace.Operations;

public abstract class UnaryOperation(DataType dataType, Operation argument)
    : Operation(dataType, argument.DependsOnVariables, argument.IsIdempotent)
{
    public Operation Argument { get; internal set; } = argument;

    public abstract double Evaluate(double argument);
    public abstract Expression GenerateExpression(Expression argument);
}