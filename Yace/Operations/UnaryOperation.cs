using System.Linq.Expressions;

namespace Yace.Operations;

public abstract class UnaryOperation(DataType dataType, Operation argument)
    : Operation(dataType, argument.DependsOnVariables, argument.IsIdempotent)
{
    public Operation Argument { get; internal set; } = argument;

    public override Expression ExecuteDynamic(ParameterExpression contextParameter) =>
        GenerateExpression(Argument.ExecuteDynamic(contextParameter));
    public override double Execute(FormulaContext context) =>
        Evaluate(Argument.Execute(context));

    public abstract double Evaluate(double argument);
    public abstract Expression GenerateExpression(Expression argument);
}
