using System;

namespace Jace.Execution;

public record FunctionInfo(
    string FunctionName,
    int NumberOfParameters,
    bool IsIdempotent,
    bool IsReadOnly,
    bool IsDynamicFunc,
    Delegate Function)
{
    public string FunctionName { get; } = FunctionName;

    public int NumberOfParameters { get; } = NumberOfParameters;

    public bool IsReadOnly { get; } = IsReadOnly;

    public bool IsIdempotent { get; } = IsIdempotent;

    public bool IsDynamicFunc { get; } = IsDynamicFunc;

    public Delegate Function { get; } = Function;
}