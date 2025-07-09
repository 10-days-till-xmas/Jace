using System;
using System.Collections;
using System.Collections.Generic;
using Jace.Execution;

namespace Jace.Tests.Mocks
{
    public class MockFunctionRegistry(IEnumerable<string> functionNames) : IFunctionRegistry
    {
        private readonly HashSet<string> functionNames = [..functionNames];

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
            return new FunctionInfo(functionName, 1, true, false, false, null!);
        }

        public bool IsFunctionName(string functionName)
        {
            return functionNames.Contains(functionName);
        }

        public FunctionInfo this[string functionName]
        {
            get => functionNames.Contains(functionName) ? GetFunctionInfo(functionName) : throw new KeyNotFoundException($"Function '{functionName}' not found in the registry.");
            set => throw new NotImplementedException();
        }

        public bool Contains(string functionName)
        {
            return functionNames.Contains(functionName);
        }

        public bool TryGetFunctionInfo(string functionName, out FunctionInfo functionInfo)
        {
            throw new NotImplementedException();
        }

        public void RegisterFunction(string functionName, Delegate function, bool isIdempotent = true, bool isReadOnly = false)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
