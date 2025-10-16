namespace Jace.Operations;

public abstract class Operation
{
    public Operation(DataType dataType, bool dependsOnVariables, bool isIdempotent)
    {
        DataType = dataType;
        DependsOnVariables = dependsOnVariables;
        IsIdempotent = isIdempotent;
    }

    public DataType DataType { get; private set; }

    public bool DependsOnVariables { get; internal set; }

    public bool IsIdempotent { get; private set; }
}