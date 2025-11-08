using System.Collections.Generic;
using Yace.Execution;

namespace Yace;

public sealed class FormulaContext(
    IDictionary<string, double> variables,
    IFunctionRegistry functionRegistry,
    IConstantRegistry constantRegistry)
{
    public IDictionary<string, double> Variables { get; } = variables;

    public IFunctionRegistry FunctionRegistry { get; } = functionRegistry;
    public IConstantRegistry ConstantRegistry { get; } = constantRegistry;
}
