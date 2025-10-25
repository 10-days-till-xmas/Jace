using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Execution;
using static System.Linq.Expressions.Expression;
namespace Jace.Operations;

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
        return Invoke(function, args);
    }

    private static double Invoke(Delegate function, double[] args)
    {
        return function switch
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            Func<double> func0
                => func0.Invoke(),
            Func<double, double> func1
                => func1.Invoke(args[0]),
            Func<double, double, double> func2
                => func2.Invoke(args[0], args[1]),
            Func<double, double, double, double> func3
                => func3.Invoke(args[0], args[1], args[2]),
            Func<double, double, double, double, double> func4
                => func4.Invoke(args[0], args[1], args[2], args[3]),
            Func<double, double, double, double, double, double> func5
                => func5.Invoke(args[0], args[1], args[2], args[3], args[4]),
            Func<double, double, double, double, double, double, double> func6
                => func6.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]),
            Func<double, double, double, double, double, double, double, double> func7
                => func7.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
            Func<double, double, double, double, double, double, double, double, double> func8 =>
                func8.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
            Func<double, double, double, double, double, double, double, double, double, double> func9 =>
                func9.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
            Func<double, double, double, double, double, double, double, double, double, double, double> func10 =>
                func10.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double> func11
                => func11.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double> func12
                => func12.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> func13
                => func13.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func14
                => func14.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func15
                => func15.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func16
                => func16.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]),
            DynamicFunc<double, double> dynamicFunc => dynamicFunc.Invoke(args),
            _ => (double)function.DynamicInvoke(args)!
        };
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