using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Yace.Execution;
using Yace.Util;
using static System.Linq.Expressions.Expression;
namespace Yace.Operations;

public sealed class Function(DataType dataType, string name, IList<Operation> arguments, bool isIdempotent)
    : Operation(dataType, arguments.Any(static o => o.DependsOnVariables),
                isIdempotent && arguments.All(static o => o.IsIdempotent))
{
    private IList<Operation> _arguments = arguments;

    public string FunctionName { get; } = name;

    public IList<Operation> Arguments {
        get => _arguments;
        internal set
        {
            _arguments = value;
            DependsOnVariables = _arguments.Any(static o => o.DependsOnVariables);
        }
    }

    public override double Execute(FormulaContext context)
    {
        if (context.FunctionRegistry is null)
            throw new InvalidOperationException("Cannot execute a function without a function registry.");
        if (!context.FunctionRegistry.TryGetInfo(FunctionName, out var info))
            throw new KeyNotFoundException($"Function '{FunctionName}' not found.");
        var args = Arguments.Select(arg => arg.Execute(context))
                            .ToArray();
        var function = info.Function;
        return FuncUtil.Invoke(function, args);
    }
    
    private static readonly ParameterExpression infoVar = Variable(typeof(FunctionInfo), "info");
    private static readonly ParameterExpression argsVar = Variable(typeof(double[]), "args");
    private static readonly ConstantExpression nullConst = Constant(null);
    private static readonly UnaryExpression throwInvalidOpEx = Throw(New(
        typeof(InvalidOperationException).GetConstructor([typeof(string)])!,
        Constant("Cannot execute a function without a function registry.")
    ));
    private static readonly MethodInfo tryGetInfoMethod = typeof(IRegistry<FunctionInfo>)
       .GetMethod(nameof(IRegistry<FunctionInfo>.TryGetInfo))!;
    private static readonly ConstructorInfo keyNotFoundExConstructor = typeof(KeyNotFoundException)
       .GetConstructor([typeof(string)])!;
    public override Expression ExecuteDynamic(ParameterExpression contextParameter)
    {
        // context.FunctionRegistry
        var functionRegistryProp = Property(contextParameter, nameof(FormulaContext.FunctionRegistry));

        // if (context.FunctionRegistry is null) throw new InvalidOperationException(...)
        var nullCheck = Equal(functionRegistryProp, nullConst);
        var ifNullThrow = IfThen(nullCheck, throwInvalidOpEx);

        // if (!context.FunctionRegistry.TryGetInfo(FunctionName, out var info)) throw new KeyNotFoundException(...)
        var tryGetCall = Call(functionRegistryProp, tryGetInfoMethod, Constant(FunctionName), infoVar);
        var keyNotFoundEx = New(keyNotFoundExConstructor, Constant($"Function '{FunctionName}' not found."));
        var ifTryGetFail = IfThen(Not(tryGetCall), Throw(keyNotFoundEx));
        
        // Arguments.Select(arg => arg.Execute(context)).ToArray()
        var argExpressions = Arguments.Select(arg => arg.ExecuteDynamic(contextParameter)).ToArray();
        var assignArgs = Assign(argsVar, NewArrayInit(typeof(double), argExpressions));
        
        // FuncUtil.Invoke(info.Function, args)
        var invokeCall = Call(
            invokeMethod,
            infoVar_Function,
            argsVar
        );
        
        // Build the block
        return Block(
            [infoVar, argsVar],
            ifNullThrow,
            ifTryGetFail,
            assignArgs,
            invokeCall
        );
    }

    internal Expression AsExpression(IFunctionRegistry functionRegistry, ParameterExpression contextParameter, Func<Operation, Expression> compiler)
    {
        var functionInfo = functionRegistry.GetInfo(FunctionName);
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
       .GetRuntimeMethod(nameof(IFunctionRegistry.GetInfo), [typeof(string)])!;

    private static readonly MethodInfo invokeMethod = typeof(FuncUtil).GetMethod(nameof(FuncUtil.Invoke),
                                                          BindingFlags.NonPublic | BindingFlags.Static
                                                          #if !NETSTANDARD2_0
                                                        , [typeof(Delegate), typeof(double[])]
                                                          #endif
                                                      ) ?? throw new InvalidOperationException("Could not find FuncUtil.Invoke method.");

    private static readonly MemberExpression infoVar_Function = Property(infoVar, nameof(FunctionInfo.Function));

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
