using System;

namespace Jace.Execution;

public record FunctionInfo(
    string Name,
    int NumberOfParameters,
    bool IsIdempotent,
    bool IsReadOnly,
    bool IsDynamicFunc,
    Delegate Function) : InfoItemBase(Name, IsReadOnly)
{
    public int NumberOfParameters { get; } = NumberOfParameters;

    public bool IsIdempotent { get; } = IsIdempotent;

    public bool IsDynamicFunc { get; } = IsDynamicFunc;

    public Delegate Function { get; } = Function;
}