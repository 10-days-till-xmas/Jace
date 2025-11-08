using System;
using System.Runtime.InteropServices;

namespace Jace.Benchmark;
[StructLayout(LayoutKind.Explicit)]
public readonly struct IntDoubleUnion
{
    [FieldOffset(1)]
    private readonly double _doubleValue;
    [FieldOffset(1)]
    private readonly int _intValue;
    [field:FieldOffset(0)]
    public DataType Type { get; } // backing field is a byte
    public double DoubleValue => Type is DataType.FloatingPoint
                                     ? _doubleValue
                                     : throw new InvalidOperationException("Value is not a double.");
    public int IntValue => Type is DataType.Integer
                               ? _intValue
                               : throw new InvalidOperationException("Value is not an integer.");
    public IntDoubleUnion(double value)
    {
        _doubleValue = value;
        Type = DataType.FloatingPoint;
    }

    public IntDoubleUnion(int value)
    {
        _intValue = value;
        Type = DataType.Integer;
    }
}