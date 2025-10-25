using System;
using System.Collections.Generic;
using Jace.Operations;

namespace Jace.Execution;

public interface IExecutor : IUsesText
{
    double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, IDictionary<string, double>? variables = null);

    Func<IDictionary<string, double>, double> BuildFormula(Operation operation, IFunctionRegistry? functionRegistry, IConstantRegistry? constantRegistry);
}