using System;
using JetBrains.Annotations;

namespace Jace.Execution;

[PublicAPI]
public sealed record ConstantInfo(string Name, double Value, bool IsReadOnly = false)
    : InfoItemBase(Name, IsReadOnly), IEquatable<ConstantInfo>, IEquatable<double>, IComparable<double>
{
    public double Value { get; } = Value;
    
    public static explicit operator double(ConstantInfo constantInfo) => constantInfo.Value;
    
    public int CompareTo(double other) => Value.CompareTo(other);
    
    public bool Equals(double other) => Value.Equals(other);
}