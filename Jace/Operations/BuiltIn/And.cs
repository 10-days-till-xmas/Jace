using System.Linq.Expressions;

namespace Jace.Operations.BuiltIn;

public sealed class And(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Convert(
            Expression.And(
                Expression.NotEqual(argument1, Expression.Constant(0.0)), 
                Expression.NotEqual(argument2, Expression.Constant(0.0))),
            typeof(double));
    }

    protected override double Calculate(double argument1, double argument2)
    {
        var arg1 = argument1 != 0.0;
        var arg2 = argument2 != 0.0;
        return (arg1 && arg2) ? 1.0 : 0.0;
    }
}

