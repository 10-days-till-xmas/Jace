using System;
using System.Linq.Expressions;

namespace Jace.Operations;

public abstract class BinaryOperation(DataType dataType, Operation argument1, Operation argument2)
    : Operation(dataType, argument1.DependsOnVariables || argument2.DependsOnVariables,
                argument1.IsIdempotent && argument2.IsIdempotent)
{
    public Operation Argument1 { get; internal set; } = argument1;
    public Operation Argument2 { get; internal set; } = argument2;

    public abstract double Evaluate(double argument1, double argument2);
    public abstract Expression GenerateExpression(Expression argument1, Expression argument2);
}