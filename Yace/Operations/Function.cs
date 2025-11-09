using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Yace.Execution;
using Yace.Util;
using static System.Linq.Expressions.Expression;
namespace Yace.Operations;

public sealed class Function(DataType dataType, string functionName, IList<Operation> arguments, bool isIdempotent)
    : Operation(dataType, arguments.Any(static o => o.DependsOnVariables),
                isIdempotent && arguments.All(static o => o.IsIdempotent))
{
    private IList<Operation> _arguments = arguments;

    public string FunctionName { get; private set; } = functionName;

    public IList<Operation> Arguments {
        get => _arguments;
        internal set
        {
            _arguments = value;
            DependsOnVariables = _arguments.FirstOrDefault(o => o.DependsOnVariables) != null;
        }
    }

    internal double Invoke(IFunctionRegistry functionRegistry, Func<Operation, double> executor)
    {
        var functionInfo = functionRegistry.GetFunctionInfo(FunctionName);
        var args = Arguments.Select(executor)
                            .ToArray();
        var function = functionInfo.Function;
        return FuncUtil.Invoke(function, args);
    }

    internal Expression AsExpression(IFunctionRegistry functionRegistry, ParameterExpression contextParameter, Func<Operation, Expression> compiler)
    {
        var functionInfo = functionRegistry.GetFunctionInfo(FunctionName);
        Type funcType;
        Type[] parameterTypes;
        var arguments = Arguments.Select(compiler).ToArray();
        if (functionInfo.IsDynamicFunc)
        {
            funcType = typeof(DynamicFunc<double, double>);
            parameterTypes = [typeof(double[])];
            arguments = [NewArrayInit(typeof(double), arguments)];
        }
        else
        {
            funcType = GetFuncType(functionInfo.NumberOfParameters);
            parameterTypes = Enumerable.Repeat(typeof(double), functionInfo.NumberOfParameters)
                                       .ToArray();
        }

        var getFunctionRegistry = Property(contextParameter, "FunctionRegistry");
        Expression funcInstance;
        if (functionInfo.IsOverWritable)
        {
            // The function could be overwritten, so this could be a different instance each time
            // However, this is intended behavior (e.g., for user-defined functions, prevents the need to rebuild the expression tree)
            funcInstance = Constant(functionInfo.Function, funcType);
        }
        else
        {
            funcInstance = Convert(Property(Call(getFunctionRegistry,
                                                 m_IFunctionRegistry_GetFunctionInfo,
                                                 Constant(FunctionName)),
                                            nameof(FunctionInfo.Function)),
                                   funcType);
        }

        return Call(funcInstance,
                    funcType.GetRuntimeMethod(nameof(Func<double>.Invoke), parameterTypes)!,
                    arguments);
    }

    private static readonly string FuncAssemblyQualifiedName =
        typeof(Func<double, double, double, double, double, double, double, double, double, double>).GetTypeInfo().Assembly.FullName
     ?? throw new InvalidOperationException("Could not get assembly qualified name for Func type.");

    private static readonly MethodInfo m_IFunctionRegistry_GetFunctionInfo = typeof(IFunctionRegistry)
       .GetRuntimeMethod(nameof(IFunctionRegistry.GetFunctionInfo), [typeof(string)])!;

    private static Type GetFuncType(int numberOfParameters)
    {
        var funcTypeName = numberOfParameters < 9
                               ? $"System.Func`{numberOfParameters + 1}"
                               : $"System.Func`{numberOfParameters + 1}, {FuncAssemblyQualifiedName}";
        var funcType = Type.GetType(funcTypeName);

        var typeArguments = Enumerable.Repeat(typeof(double), numberOfParameters + 1)
                                      .ToArray();

        return funcType!.MakeGenericType(typeArguments);
    }
}
