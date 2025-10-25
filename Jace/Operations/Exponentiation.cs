using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Jace.Operations;

public sealed class Exponentiation(DataType dataType, Operation argument1, Operation argument2)
    : BinaryOperation(dataType, argument1, argument2)
{
    public override double Evaluate(double argument1, double argument2)
    {
        return Math.Pow(argument1, argument2);
    }
    private static readonly MethodInfo PowMethodInfo = typeof(Math).GetRuntimeMethod(nameof(Math.Pow), [typeof(double), typeof(double)])!;
    public override Expression GenerateExpression(Expression argument1, Expression argument2)
    {
        return Expression.Call(null, PowMethodInfo, argument1, argument2);
    }
}