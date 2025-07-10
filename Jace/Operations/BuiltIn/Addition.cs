using System.Linq.Expressions;

namespace Jace.Operations.BuiltIn;

public sealed class Addition(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Add(argument1, argument2);
    }

    protected override double Calculate(double argument1, double argument2)
    {
        return argument1 + argument2;
    }
}