using System.Collections.Generic;
using System.Linq.Expressions;
using Jace.Execution;

namespace Jace.Operations;

public abstract class Operation(DataType dataType, bool dependsOnVariables, bool isIdempotent)
{
    public DataType DataType { get; private set; } = dataType;

    public bool DependsOnVariables { get; internal set; } = dependsOnVariables;

    public bool IsIdempotent { get; } = isIdempotent;

    public abstract double Execute(Interpreter interpreter, 
        IFunctionRegistry functionRegistry,
        IConstantRegistry constantRegistry,
        IDictionary<string, double> variables);

    // TODO: implement a more type-safe way to handle different operation types (float, int, boolean)
    public abstract Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, 
        ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry);
}