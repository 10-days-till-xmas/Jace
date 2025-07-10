using System.Linq.Expressions;

namespace Jace.Operations;

public sealed class Division(DataType dataType, Operation dividend, Operation divisor) 
    : BinaryOperation(dataType, dividend, divisor)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Divide(argument1, argument2);
    }

    protected override double Calculate(double argument1, double argument2)
    {
        return argument1 / argument2;
    }
}