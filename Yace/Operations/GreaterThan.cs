using System.Linq.Expressions;
using Yace.Util;

namespace Yace.Operations;

public sealed class GreaterThan(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    public override double Evaluate(double argument1, double argument2)
    {
        return (argument1 > argument2).AsDouble();
    }

    public override Expression GenerateExpression(Expression argument1, Expression argument2)
    {
        return Expression.GreaterThan(argument1, argument2).AsDouble();
    }
}