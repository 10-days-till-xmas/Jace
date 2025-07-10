using System.Linq.Expressions;

namespace Jace.Operations.BuiltIn;

public sealed class LessOrEqualThan(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Convert(Expression.LessThanOrEqual(argument1, argument2),
                typeof(double));
    }

    protected override double Calculate(double argument1, double argument2)
    {
        return (argument1 <= argument2) ? 1.0 : 0.0;
    }
}