namespace Yace.Operations;

/// <summary>
/// Represents a variable in a mathematical formula.
/// </summary>
public sealed class Variable(string name) : Operation(DataType.FloatingPoint, true, false)
{
    public string Name { get; } = name;

    public override bool Equals(object? obj) => obj is Variable other
                                             && Name.Equals(other.Name);

    public override int GetHashCode() => Name.GetHashCode();
}