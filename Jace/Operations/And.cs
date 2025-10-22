using System.Linq.Expressions;
using Jace.Util;

namespace Jace.Operations;

public sealed class And(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    public override double Evaluate(double argument1, double argument2)
    {
        return (argument1.AsBool() && argument2.AsBool()).AsDouble();
    }

    public override Expression GenerateExpression(Expression argument1, Expression argument2)
    {
        return Expression.And(argument1.AsBool(), argument2.AsBool()).AsDouble();
    }
}