using System;
using System.Linq.Expressions;
using Yace.Operations;

namespace Yace.Tests.Mocks;

public sealed class Sqrt(Operation argument) : UnaryOperation(DataType.FloatingPoint, argument)
{
    public override double Evaluate(double argument)
    {
        return Math.Sqrt(argument);
    }

    public override Expression GenerateExpression(Expression argument)
    {
        return Expression.Call(typeof(Math).GetMethod(nameof(Math.Sqrt), [typeof(double)])!, argument);
    }
}
