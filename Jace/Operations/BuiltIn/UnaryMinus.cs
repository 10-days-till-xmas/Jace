using System.Linq.Expressions;

namespace Jace.Operations.BuiltIn;

public sealed class UnaryMinus(DataType dataType, Operation argument)
    : UnaryOperation(dataType, argument)
{
    protected override double Calculate(double argument)
    {
        return -argument;
    }

    protected override Expression ExpressionOperation(Expression argument)
    {
        return Expression.Negate(argument);
    }
}