using System.Linq.Expressions;

namespace Jace.Operations;

public sealed class Division(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    public override double Evaluate(double argument1, double argument2)
    {
        return argument1 / argument2;
    }

    public override Expression GenerateExpression(Expression argument1, Expression argument2)
    {
        return Expression.Divide(argument1, argument2);
    }
}