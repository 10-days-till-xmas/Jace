using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Yace.Interfaces;
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
        var funcRegistry = context.FunctionRegistry as ReadOnlyFunctionRegistry
                        ?? new ReadOnlyFunctionRegistry(context.FunctionRegistry, CaseSensitive);
        var constantRegistry = context.ConstantRegistry as ReadOnlyConstantRegistry
                            ?? new ReadOnlyConstantRegistry(context.ConstantRegistry, CaseSensitive);
        return BuildFormula(operation, funcRegistry, constantRegistry)(variables);
    }

    public Func<IDictionary<string, double>, double> BuildFormulaCapturing(Operation operation,
                                                                  IFunctionRegistry? functionRegistry,
                                                                  IConstantRegistry? constantRegistry)
    {
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
    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
        ReadOnlyFunctionRegistry? functionRegistry,
        ReadOnlyConstantRegistry? constantRegistry)
    {
        functionRegistry ??= ReadOnlyFunctionRegistry.Empty;
        constantRegistry ??= ReadOnlyConstantRegistry.Empty;

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
}
