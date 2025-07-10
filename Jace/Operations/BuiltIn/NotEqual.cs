using System;
using System.Linq.Expressions;

namespace Jace.Operations.BuiltIn;

public sealed class NotEqual(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Convert(Expression.NotEqual(argument1, argument2),
            typeof(double));
    }

    protected override double Calculate(double argument1, double argument2)
    {
        const double TOLERANCE = 1e-10; // TODO: Make this configurable (through JaceOptions?)
        return (Math.Abs(argument1 - argument2) > TOLERANCE) ? 1.0 : 0.0; 
    }
}