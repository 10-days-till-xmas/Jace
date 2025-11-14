using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ParameterInfo = Yace.Execution.ParameterInfo;

namespace Yace.Util;

/// <summary>
/// An adapter for creating a func wrapper around a func accepting a dictionary. The wrapper
/// can create a func that has an argument for every expected key in the dictionary.
/// </summary>
public static class FuncAdapter
{
    private static readonly MethodInfo m_Dictionary_Add =
        typeof(Dictionary<string, double>).GetRuntimeMethod(nameof(Dictionary<string, double>.Add), [typeof(string), typeof(double)])!;

    /// <summary>
    /// Wrap the parsed the function into a delegate of the specified type. The delegate must accept
    /// the parameters defined in the parameter collection. The order of parameters is respected as defined
    /// in the parameter collection.
    /// <br/>
    /// The function must accept a dictionary of strings and doubles as input. The values passed to the
    /// wrapping function will be passed to the function using the dictionary. The keys in the dictionary
    /// are the parameter names of the wrapping function.
    /// </summary>
    /// <param name="parameters">The required parameters of the wrapping function delegate.</param>
    /// <param name="function">The function that must be wrapped.</param>
    /// <returns>A delegate instance of the required type.</returns>
    public static Delegate Wrap(IEnumerable<ParameterInfo> parameters,
                                Func<IDictionary<string, double>, double> function)
    {
        var parameterArray = parameters.ToArray();

        return GenerateDelegate(parameterArray, function);
    }

    private static Delegate GenerateDelegate(ParameterInfo[] parameterArray,
                                             Func<Dictionary<string, double>, double> function)
    {
        var delegateType = GetDelegateType(parameterArray);
        var dictionaryType = typeof(Dictionary<string, double>);
        var dictionaryExpression =
            Expression.Variable(typeof(Dictionary<string, double>), "dictionary");
        var dictionaryAssignExpression =
            Expression.Assign(dictionaryExpression, Expression.New(dictionaryType));

        var methodBody = new List<Expression> { dictionaryAssignExpression };

        var parameterExpressions = parameterArray.Select((info) =>
        { // Create parameter expression for each func parameter
            var parameterType = info.DataType == DataType.FloatingPoint ? typeof(double) : typeof(int);
            var paramExpr = Expression.Parameter(parameterType, info.Name);

            methodBody.Add(Expression.Call(dictionaryExpression,
                                           m_Dictionary_Add,
                                           Expression.Constant(info.Name),
                                           Expression.Convert(paramExpr, typeof(double)))
                          );
            return paramExpr;
        }).ToArray();

        var invokeExpression = Expression.Invoke(Expression.Constant(function), dictionaryExpression);
        methodBody.Add(invokeExpression);

        var lambdaExpression = Expression.Lambda(delegateType,
                                                 Expression.Block([dictionaryExpression], methodBody),
                                                 parameterExpressions);

        return lambdaExpression.Compile();
    }

    private static Type GetDelegateType(ParameterInfo[] parameters)
    {
        var funcTypeName = $"System.Func`{parameters.Length + 1}";
        var funcType = Type.GetType(funcTypeName);

        var typeArguments = new Type[parameters.Length + 1];
        for (var i = 0; i < parameters.Length; i++)
            typeArguments[i] = (parameters[i].DataType == DataType.FloatingPoint) ? typeof(double) : typeof(int);
        typeArguments[typeArguments.Length - 1] = typeof(double);

        return funcType!.MakeGenericType(typeArguments);
    }
}
