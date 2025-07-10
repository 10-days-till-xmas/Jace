using System;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Operations;

namespace Jace.Tests.Mocks.Extensibility;

public sealed class CalculateHypotenuse(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Call(
            Expression.Constant(this),
            typeof(CalculateHypotenuse).GetMethod(nameof(Calculate), 
                BindingFlags.Instance | BindingFlags.NonPublic)!,
            argument1,
            argument2);
    }

    protected override double Calculate(double argument1, double argument2)
    {
        return Math.Sqrt(argument1 * argument1 + argument2 * argument2);
    }
}