using System;

namespace Yace.Execution;

public sealed record FunctionInfo(
    string Name,
    int NumberOfParameters,
    bool IsIdempotent,
    bool IsOverWritable,
    bool IsDynamicFunc,
    Delegate Function) : RegistryItem(Name, IsOverWritable);
