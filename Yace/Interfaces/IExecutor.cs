using System;
using System.Collections.Generic;
using Yace.Operations;

namespace Yace.Execution;

public interface IExecutor : IUsesText
{
    double Execute(Operation operation, FormulaContext context);

    Func<IDictionary<string, double>, double> BuildFormulaCapturing(Operation operation, IFunctionRegistry? functionRegistry, IConstantRegistry? constantRegistry);
    Func<IDictionary<string, double>, double> BuildFormula(Operation operation, ReadOnlyFunctionRegistry? functionRegistry, ReadOnlyConstantRegistry? constantRegistry);
}
