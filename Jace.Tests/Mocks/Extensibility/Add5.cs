using System.Linq.Expressions;
using Jace.Operations;

namespace Jace.Tests.Mocks.Extensibility;

public sealed class Add5(DataType dataType, Operation argument) 
    : UnaryOperation(dataType, argument)
{
    protected override double Calculate(double argument)
    {
        return argument + 5;
    }

    protected override Expression ExpressionOperation(Expression argument)
    {
        return Expression.Add(argument, Expression.Constant(5.0));
    }
}