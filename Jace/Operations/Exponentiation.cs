using System;
using System.Linq.Expressions;

namespace Jace.Operations;

public sealed class Exponentiation(DataType dataType, Operation @base, Operation exponent) 
    : BinaryOperation(dataType, @base, exponent)
{
    protected override Expression ExpressionOperation(Expression argument1, Expression argument2)
    {
        return Expression.Power(argument1, argument2);
    }

    protected override double Calculate(double argument1, double argument2)
    {
        return Math.Pow(argument1, argument2);
    }
}