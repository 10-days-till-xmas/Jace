using System.Collections.Generic;
using Yace.Execution;

namespace Yace;

public sealed class FormulaContext(
    IFunctionRegistry? functionRegistry,
    IConstantRegistry? constantRegistry,
    IDictionary<string, double>? variables = null)
{
    public IDictionary<string, double>? Variables { get; } = variables;

    public IFunctionRegistry? FunctionRegistry { get; } = functionRegistry;
    public IConstantRegistry? ConstantRegistry { get; } = constantRegistry;
    public static implicit operator FormulaContext((IFunctionRegistry? functionRegistry, IConstantRegistry? constantRegistry) tuple)
        => new (tuple.functionRegistry, tuple.constantRegistry);
}
