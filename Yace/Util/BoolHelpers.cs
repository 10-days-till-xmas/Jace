using System.Linq.Expressions;

namespace Yace.Util;

internal static class BoolHelpers
{
    public static bool AsBool(this double value) => value != 0;

    public static BinaryExpression AsBool(this Expression value) =>
        Expression.NotEqual(value, Expression.Constant(0.0));

    public static double AsDouble(this bool value) => value ? 1 : 0;

    public static Expression AsDouble(this BinaryExpression value) =>
        Expression.Condition(
            value,
            Expression.Constant(1.0),
            Expression.Constant(0.0));
}