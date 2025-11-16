using System;
using JetBrains.Annotations;
using Yace.Execution;

namespace Yace.Interfaces;

[PublicAPI]
public interface IFunctionRegistry : IRegistry<FunctionInfo>
{
    void Register(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true);
}
