using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Execution;
using Jace.Util;

namespace Jace.Operations.BuiltIn;

public sealed class Function(DataType dataType, string functionName, IList<Operation> arguments, bool isIdempotent)
    : Operation(dataType, arguments.FirstOrDefault(o => o.DependsOnVariables) != null,
        isIdempotent && arguments.All(o => o.IsIdempotent))
{
    private IList<Operation> arguments = arguments;

    public string FunctionName { get; } = functionName;

    public IList<Operation> Arguments {
        get => arguments;
        internal set
        {
            arguments = value;
            DependsOnVariables = arguments.FirstOrDefault(o => o.DependsOnVariables) != null;
        }
    }

    public override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        var functionInfo = functionRegistry[FunctionName];

        var args = new double[functionInfo.IsDynamicFunc
            ? Arguments.Count
            : functionInfo.NumberOfParameters];
                    
        for (var i = 0; i < args.Length; i++)
            args[i] = interpreter.Execute(Arguments[i], functionRegistry, constantRegistry, variables);

        return functionInfo.Function.FastInvoke(args);
    }

    public override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        var functionInfo = functionRegistry[FunctionName];
        Type funcType;
        Type[] parameterTypes;
        Expression[] args;

        if (functionInfo.IsDynamicFunc)
        {
            funcType = typeof(DynamicFunc<double, double>);
            parameterTypes = [typeof(double[])];

            var arrayArguments = new Expression[Arguments.Count];
            for (var i = 0; i < Arguments.Count; i++)
                arrayArguments[i] = dynamicCompiler.GenerateMethodBody(Arguments[i], contextParameter, functionRegistry);
            
            args =
            [
                Expression.NewArrayInit(typeof(double), arrayArguments)
            ];
        }
        else
        {
            funcType = DelegateExtensions.GetFuncType(functionInfo.NumberOfParameters);
            parameterTypes = (from i in Enumerable.Range(0, functionInfo.NumberOfParameters)
                select typeof(double)).ToArray();

            args = new Expression[functionInfo.NumberOfParameters];
            for (var i = 0; i < functionInfo.NumberOfParameters; i++)
                args[i] = dynamicCompiler.GenerateMethodBody(Arguments[i], contextParameter, functionRegistry);
        }

        Expression getFunctionRegistry = Expression.Property(contextParameter, "FunctionRegistry");

        Expression funcInstance;
        if (functionInfo.IsReadOnly)
        {
            funcInstance = Expression.Convert(
                Expression.Property(
                    Expression.Property(
                        getFunctionRegistry,
                        typeof(IFunctionRegistry).GetProperty("Item")!,
                        Expression.Constant(FunctionName)),
                    "Function"),
                funcType);
        }
        else
            funcInstance = Expression.Constant(functionInfo.Function, funcType);

        return Expression.Call(
            funcInstance,
            funcType.GetRuntimeMethod("Invoke", parameterTypes) 
            ?? throw new InvalidOperationException($"Could not find method 'Invoke' on type '{funcType.FullName}' with parameters {string.Join(", ", parameterTypes.Select(t => t.Name))}."),
            args);
    }
}