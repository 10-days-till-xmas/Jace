namespace Jace.Operations;

public class UnaryMinus : Operation
{
    public UnaryMinus(DataType dataType, Operation argument)
        : base(dataType, argument.DependsOnVariables, argument.IsIdempotent)
    {
        Argument = argument;
    }

    public Operation Argument { get; internal set; }
}