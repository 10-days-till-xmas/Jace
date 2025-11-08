using System.Collections.Generic;
using Jace.Execution;

namespace Jace;

public sealed class FormulaContext(
    IDictionary<string, double> variables,
    IFunctionRegistry functionRegistry,
    IConstantRegistry constantRegistry)
{
    public IDictionary<string, double> Variables { get; } = variables;

    public IFunctionRegistry FunctionRegistry { get; } = functionRegistry;
    public IConstantRegistry ConstantRegistry { get; } = constantRegistry;
}
