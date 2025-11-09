using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Yace.Execution;

[PublicAPI]
public interface IFunctionRegistry : IEnumerable<FunctionInfo>, IUsesText
{
    StringComparer Comparer { get; }
    FunctionInfo GetFunctionInfo(string functionName);
    bool TryGetFunctionInfo(string functionName, [NotNullWhen(true)] out FunctionInfo? functionInfo);
    bool ContainsFunctionName(string functionName);
    void RegisterFunction(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true);
    void RegisterFunction(FunctionInfo functionInfo);
}
