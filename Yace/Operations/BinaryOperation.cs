using System.Linq.Expressions;

namespace Yace.Operations;

public abstract class BinaryOperation(DataType dataType, Operation argument1, Operation argument2)
    : Operation(dataType, argument1.DependsOnVariables || argument2.DependsOnVariables,
                argument1.IsIdempotent && argument2.IsIdempotent)
{
    public Operation Argument1 { get; internal set; } = argument1;
    public Operation Argument2 { get; internal set; } = argument2;

    public override Expression ExecuteDynamic(ParameterExpression contextParameter)
    {
        var arg1Expr = Argument1.ExecuteDynamic(contextParameter);
        var arg2Expr = Argument2.ExecuteDynamic(contextParameter);
        return GenerateExpression(arg1Expr, arg2Expr);
    }
    public override double Execute(FormulaContext context)
    {
        var arg1Value = Argument1.Execute(context);
        var arg2Value = Argument2.Execute(context);
        return Evaluate(arg1Value, arg2Value);
    }
    
    public abstract double Evaluate(double argument1, double argument2);
    public abstract Expression GenerateExpression(Expression argument1, Expression argument2);
}
