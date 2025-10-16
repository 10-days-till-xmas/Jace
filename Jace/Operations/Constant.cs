namespace Jace.Operations;

public abstract class Constant<T> : Operation
{
    public Constant(DataType dataType, T value)
        : base(dataType, false, true)
    {
        Value = value;
    }

    public T Value { get; private set; }

    public override bool Equals(object? obj)
    {
        if (obj is Constant<T> other)
            return Value!.Equals(other.Value);
        return false;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public class IntegerConstant : Constant<int>
{
    public IntegerConstant(int value)
        : base(DataType.Integer, value)
    {
    }
}

public class FloatingPointConstant : Constant<double>
{
    public FloatingPointConstant(double value)
        : base(DataType.FloatingPoint, value)
    {
    }
}