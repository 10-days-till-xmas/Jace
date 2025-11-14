using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Yace.Operations;

namespace Yace.Execution;

[PublicAPI]
public sealed class DynamicCompiler(bool caseSensitive) : IExecutor
{
    public bool CaseSensitive { get; } = caseSensitive;
    public DynamicCompiler(): this(false) { }

    public double Execute(Operation operation, FormulaContext? context = null)
    {
        var variables = context!.Variables ?? new Dictionary<string, double>();
        return BuildFormula(operation, context.FunctionRegistry, context.ConstantRegistry)(variables);
    }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry? functionRegistry,
                                                                  IConstantRegistry? constantRegistry)
    {
        functionRegistry = functionRegistry is null
                               ? ReadOnlyFunctionRegistry.Empty
                               : new ReadOnlyFunctionRegistry(functionRegistry);
        constantRegistry = constantRegistry is null
                               ? ReadOnlyConstantRegistry.Empty
                               : new ReadOnlyConstantRegistry(constantRegistry);

        var func = BuildFormulaInternal(operation);
        return CaseSensitive
                   ? variables => func(new FormulaContext(functionRegistry, constantRegistry, variables))
                   : variables =>
                   {
                       variables = new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase);
                       var context = new FormulaContext(functionRegistry, constantRegistry, variables);
                       return func(context);
                   };
    }

    private static Func<FormulaContext, double> BuildFormulaInternal(Operation operation)
    {
        var contextParameter = Expression.Parameter(typeof(FormulaContext), "context");
        return Expression.Lambda<Func<FormulaContext, double>>(operation.ExecuteDynamic(contextParameter),
                                                               contextParameter)
                         .Compile();
    }



    private static Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter)
    {
        return operation.ExecuteDynamic(contextParameter);
    }
    private static double GetVariableValueOrThrow(string variableName, FormulaContext context)
    {
        if (context.Variables!.TryGetValue(variableName, out var variable))
            return variable;
        if (context.ConstantRegistry!.TryGetConstantInfo(variableName, out var info))
            return info.Value;
        throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
    }
}
