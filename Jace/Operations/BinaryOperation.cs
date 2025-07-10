using System.Collections.Generic;
using System.Linq.Expressions;
using Jace.Execution;

namespace Jace.Operations;

public abstract class BinaryOperation(DataType dataType, Operation argument1, Operation argument2)
    : Operation(dataType, argument1.DependsOnVariables || argument2.DependsOnVariables,
        argument1.IsIdempotent && argument2.IsIdempotent)
{
    public Operation Argument1 { get; internal set; } = argument1;
    public Operation Argument2 { get; internal set; } = argument2;
    
    protected abstract double Calculate(double argument1, double argument2);
    
    // TODO: consider making this method virtual, and then use the following implementation:
    //  return Expression.Call(
    //      Expression.Constant(this),
    //      this.GetType().GetMethod(nameof(Calculate), BindingFlags.Instance | BindingFlags.NonPublic)!,
    //      argument1,
    //      argument2);
    protected abstract Expression ExpressionOperation(Expression argument1, Expression argument2);

    public sealed override double Execute(Interpreter interpreter, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
        IDictionary<string, double> variables)
    {
        return Calculate(
            interpreter.Execute(Argument1, functionRegistry, constantRegistry, variables),
            interpreter.Execute(Argument2, functionRegistry, constantRegistry, variables));
    }
    
    public sealed override Expression GenerateMethodBody(DynamicCompiler dynamicCompiler, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        var arg1 = dynamicCompiler.GenerateMethodBody(Argument1, contextParameter, functionRegistry);
        var arg2 = dynamicCompiler.GenerateMethodBody(Argument2, contextParameter, functionRegistry);
        return ExpressionOperation(arg1, arg2);
    }
}