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
    private NewExpression? keyNotFoundEx;

    // FuncUtil.Invoke(info.Function, args)
    private static readonly MethodCallExpression invokeCall = Call(
        typeof(FuncUtil).GetMethod(nameof(FuncUtil.Invoke),
            BindingFlags.NonPublic | BindingFlags.Static
            #if !NETSTANDARD2_0
          , [typeof(Delegate), typeof(double[])]
            #endif
        ) ?? throw new InvalidOperationException("Could not find FuncUtil.Invoke method."),
        Property(infoVar, nameof(FunctionInfo.Function)),
        argsVar
    );
    public override Expression ExecuteDynamic(ParameterExpression contextParameter)
    {
        // context.FunctionRegistry
        var functionRegistryProp = Property(contextParameter, nameof(FormulaContext.FunctionRegistry));

        // if (context.FunctionRegistry is null) throw new InvalidOperationException(...)
        var nullCheck = Equal(functionRegistryProp, nullConst);
        var ifNullThrow = IfThen(nullCheck, throwInvalidOpEx);

        // if (!context.FunctionRegistry.TryGetInfo(FunctionName, out var info)) throw new KeyNotFoundException(...)
        var tryGetCall = Call(functionRegistryProp, tryGetInfoMethod, Constant(FunctionName), infoVar);
        keyNotFoundEx ??= New(keyNotFoundExConstructor, Constant($"Function '{FunctionName}' not found."));
        var ifTryGetFail = IfThen(Not(tryGetCall), Throw(keyNotFoundEx));

        // Arguments.Select(arg => arg.Execute(context)).ToArray()
        var argExpressions = Arguments.Select(arg => arg.ExecuteDynamic(contextParameter)).ToArray();
        var assignArgs = Assign(argsVar, NewArrayInit(typeof(double), argExpressions));

        return Block(
            [infoVar, argsVar],
            ifNullThrow,
            ifTryGetFail,
            assignArgs,
            invokeCall
        );
    }
}
