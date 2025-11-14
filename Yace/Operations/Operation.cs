using System.Linq.Expressions;

namespace Yace.Operations;

public abstract class Operation(DataType dataType, bool dependsOnVariables, bool isIdempotent)
{
    public DataType DataType { get; } = dataType;

    public bool DependsOnVariables { get; protected set; } = dependsOnVariables;

    public bool IsIdempotent { get; } = isIdempotent;

    public abstract Expression ExecuteDynamic(ParameterExpression contextParameter);
    public abstract double Execute(FormulaContext context);
}
