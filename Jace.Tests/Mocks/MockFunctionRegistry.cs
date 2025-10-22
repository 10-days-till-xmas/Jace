using System;
using System.Collections;
using System.Collections.Generic;
using Jace.Execution;

namespace Jace.Tests.Mocks;

public class MockFunctionRegistry(IEnumerable<string> functionNames) : IFunctionRegistry
{
    public bool CaseSensitive => false;
    private HashSet<string> functionNames = [..functionNames];

    public MockFunctionRegistry()
        : this(["sin", "cos", "csc", "sec", "asin", "acos", "tan", "cot", "atan", "acot", "loge", "log10", "logn", "sqrt", "abs"
               ])
    {
    }

    public IEnumerator<FunctionInfo> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public FunctionInfo GetFunctionInfo(string functionName)
    {
        return new FunctionInfo(functionName, 1, true, false, false, null);
    }

    public bool TryGetFunctionInfo(string functionName, out FunctionInfo functionInfo)
    {
        throw new NotImplementedException();
    }

    public bool ContainsFunctionName(string functionName)
    {
        return functionNames.Contains(functionName);
    }

    public void RegisterFunction(string functionName, Delegate function)
    {
        throw new NotImplementedException();
    }

    public void RegisterFunction(string functionName, Delegate function, bool isIdempotent, bool isOverWritable)
    {
        throw new NotImplementedException();
    }

    public void RegisterFunction(FunctionInfo functionInfo)
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