namespace Jace.Operations;

public class Modulo : Operation
{
    public Modulo(DataType dataType, Operation dividend, Operation divisor)
        : base(dataType, dividend.DependsOnVariables || divisor.DependsOnVariables, dividend.IsIdempotent && divisor.IsIdempotent)
    {
        Dividend = dividend;
        Divisor = divisor;
    }

    public Operation Dividend { get; internal set; }
    public Operation Divisor { get; internal set; }
}