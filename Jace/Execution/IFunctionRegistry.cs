using System;
using System.Collections.Generic;

namespace Jace.Execution;

public interface IFunctionRegistry : IEnumerable<FunctionInfo>
{
    FunctionInfo this[string functionName] { get; set; }
    bool Contains(string functionName);
    bool TryGetFunctionInfo(string functionName, out FunctionInfo? functionInfo);
    void RegisterFunction(string functionName, Delegate function, bool isIdempotent = true, bool isReadOnly = false);
}