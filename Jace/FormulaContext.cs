using System.Collections.Generic;
using Jace.Execution;

namespace Jace;

public class FormulaContext
{
    public FormulaContext(IDictionary<string, double> variables,
                          IFunctionRegistry functionRegistry,
                          IConstantRegistry constantRegistry)
    {
        Variables = variables;
        FunctionRegistry = functionRegistry;
        ConstantRegistry = constantRegistry;
    }

    public IDictionary<string, double> Variables { get; private set; }

    public IFunctionRegistry FunctionRegistry { get; private set; }
    public IConstantRegistry ConstantRegistry { get; private set; }
}