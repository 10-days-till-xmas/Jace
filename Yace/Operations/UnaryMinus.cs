using System.Linq.Expressions;

namespace Yace.Operations;

public sealed class UnaryMinus(DataType dataType, Operation argument) : UnaryOperation(dataType, argument)
{
    public override double Evaluate(double argument)
    {
        return -argument;
    }

    public override Expression GenerateExpression(Expression argument)
    {
        return Expression.Negate(argument);
    }
}