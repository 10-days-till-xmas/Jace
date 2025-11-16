using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yace.Interfaces;
using Yace.Operations;

namespace Yace.Execution;
[PublicAPI]
public sealed class Interpreter(bool caseSensitive) : IExecutor
{
    public bool CaseSensitive { get; } = caseSensitive;
    public Interpreter(): this(false) { }

    public Func<IDictionary<string, double>, double> BuildFormulaCapturing(Operation operation,
                                                                  IFunctionRegistry? functionRegistry,
                                                                  IConstantRegistry? constantRegistry)
    {
        if (functionRegistry is not ReadOnlyFunctionRegistry)
            functionRegistry = new ReadOnlyFunctionRegistry(functionRegistry ?? new FunctionRegistry(CaseSensitive));
        if (constantRegistry is not ReadOnlyConstantRegistry)
            constantRegistry = new ReadOnlyConstantRegistry(constantRegistry ?? new ConstantRegistry(CaseSensitive));
        return CaseSensitive
                   ? variables => Execute(operation, new FormulaContext(functionRegistry, constantRegistry, variables))
                   : variables => Execute(operation, new FormulaContext(functionRegistry, constantRegistry,
                       new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase)));
    }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
        ReadOnlyFunctionRegistry? functionRegistry,
        ReadOnlyConstantRegistry? constantRegistry)
    {
        functionRegistry ??= ReadOnlyFunctionRegistry.Empty;
        constantRegistry ??= ReadOnlyConstantRegistry.Empty;
        return CaseSensitive
                   ? variables => Execute(operation, new FormulaContext(functionRegistry, constantRegistry, variables))
                   : variables => Execute(operation, new FormulaContext(functionRegistry, constantRegistry,
                       new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase)));
    }

    public double Execute(Operation operation, FormulaContext context)
    {
        return operation.Execute(context);
    }
}


