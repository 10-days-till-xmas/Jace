using System;
using JetBrains.Annotations;

namespace Yace.Execution;

[PublicAPI]
public interface IFunctionRegistry : IRegistry<FunctionInfo>
{
    void Register(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true);
}
