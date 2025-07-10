using System.Collections.Generic;
using System.Linq.Expressions;
using Jace.Execution;

namespace Jace.Operations;

public abstract class UnaryOperation(DataType dataType, Operation argument) 
    : Operation(dataType, argument.DependsOnVariables, argument.IsIdempotent)
{
    public Operation Argument { get; internal set; } = argument;

    protected abstract double Calculate(double argument);

    protected abstract Expression ExpressionOperation(Expression argument);
    
    public sealed override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        return Calculate(interpreter.Execute(Argument, functionRegistry, constantRegistry, variables));
    }

    public sealed override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        var arg1 = dynamicCompiler.GenerateMethodBody(Argument, contextParameter, functionRegistry);
        return ExpressionOperation(arg1);
    }
}