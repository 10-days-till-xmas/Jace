namespace Jace.Operations;

public abstract class Constant<T>(DataType dataType, T value)
    : Operation(dataType, false, true) where T : notnull
{
    public T Value { get; } = value;

    public override bool Equals(object? obj)
    {
        return obj is Constant<T> other
            && Value.Equals(other.Value);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}

public sealed class IntegerConstant(int value) : Constant<int>(DataType.Integer, value);

public sealed class FloatingPointConstant(double value) : Constant<double>(DataType.FloatingPoint, value);