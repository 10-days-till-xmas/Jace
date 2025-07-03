using System;
using JetBrains.Annotations;

namespace Jace.Execution;

[PublicAPI]
public sealed record ConstantInfo(string ConstantName, double Value, bool IsReadOnly)
    : IEquatable<ConstantInfo>, IEquatable<double>, IComparable<double>
{
    public string ConstantName { get; } = ConstantName;
    
    public double Value { get; } = Value;
    
    public bool IsReadOnly { get; } = IsReadOnly;
    
    public void Deconstruct(out string constantName, out double value, out bool isReadOnly)
    {
        constantName = ConstantName;
        value = Value;
        isReadOnly = IsReadOnly;
    }
    
    public static explicit operator double(ConstantInfo constantInfo) => constantInfo.Value;
    
    public int CompareTo(double other) => Value.CompareTo(other);
    
    public bool Equals(double other) => Value.Equals(other);
}