using System;
using System.Collections;
using System.Collections.Generic;
using Yace.Execution;

namespace Yace.Tests.Mocks;

public sealed class MockFunctionRegistry(IEnumerable<string> functionNames) : IFunctionRegistry
{
    public bool CaseSensitive => false;
    private readonly HashSet<string> functionNames = [..functionNames];
    public StringComparer Comparer => StringComparer.OrdinalIgnoreCase;
    public MockFunctionRegistry()
        : this(["sin", "cos", "csc", "sec", "asin", "acos", "tan", "cot", "atan", "acot", "loge", "log10", "logn", "sqrt", "abs"
               ])
    {
    }

    public IEnumerator<FunctionInfo> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public FunctionInfo GetInfo(string functionName)
    {
        return new FunctionInfo(functionName, 1, true, false, false, null!);
    }

    public bool TryGetInfo(string functionName, out FunctionInfo functionInfo)
    {
        throw new NotImplementedException();
    }

    public bool ContainsName(string functionName)
    {
        return functionNames.Contains(functionName);
    }

    public void RegisterFunction(string functionName, Delegate function)
    {
        throw new NotImplementedException();
    }

    public void Register(string functionName, Delegate function, bool isIdempotent, bool isOverWritable)
    {
        throw new NotImplementedException();
    }

    public void Register(FunctionInfo functionInfo)
    {
        throw new NotImplementedException();
    }

    public void RegisterFunction(string functionName, int numberOfParameters)
    {
        throw new NotImplementedException();
    }

    public void RegisterFunction(string functionName, Delegate function, int numberOfParameters)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
