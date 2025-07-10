using System;
using System.Collections.Generic;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution;

public sealed class Interpreter(bool caseSensitive = false) : IExecutor
{
    // TODO: this class could be made static 
    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation, 
        IFunctionRegistry functionRegistry,
        IConstantRegistry constantRegistry)
    {
        return variables =>
            Execute(operation, functionRegistry, constantRegistry, 
                caseSensitive 
                    ? variables
                    : EngineUtil.ConvertVariableNamesToLowerCase(variables));
    }

    public double Execute(Operation operation,
        IFunctionRegistry functionRegistry,
        IConstantRegistry constantRegistry,
        IDictionary<string, double>? variables)
    {
        return operation.Execute(this, functionRegistry, constantRegistry, variables ?? new Dictionary<string, double>());
    }
}