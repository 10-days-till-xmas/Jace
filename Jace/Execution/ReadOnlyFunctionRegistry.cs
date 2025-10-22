using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Jace.Util;

namespace Jace.Execution;

public sealed class ReadOnlyFunctionRegistry : IFunctionRegistry
{
    public bool CaseSensitive { get; }
    private readonly ReadOnlyDictionary<string, FunctionInfo> functions;

    public ReadOnlyFunctionRegistry(IFunctionRegistry innerFunctionRegistry)
    {
        CaseSensitive = innerFunctionRegistry.CaseSensitive;
        functions = new ReadOnlyDictionary<string, FunctionInfo>(innerFunctionRegistry.ToDictionary(f => f.FunctionName, f => f));
    }

    public IEnumerator<FunctionInfo> GetEnumerator()
    {
        return functions.Values!.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public FunctionInfo GetFunctionInfo(string functionName)
    {
        return TryGetFunctionInfo(functionName, out var functionInfo)
                   ? functionInfo
                   : throw new KeyNotFoundException(functionName);
    }
    
    public bool TryGetFunctionInfo(string functionName, [NotNullWhen(true)] out FunctionInfo? functionInfo)
    {
        return string.IsNullOrEmpty(functionName)
                   ? throw new ArgumentNullException(nameof(functionName))
                   : functions.TryGetValue(ConvertFunctionName(functionName), out functionInfo);
    }

    public void RegisterFunction(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true)
    {
        throw new ReadOnlyException("This FunctionRegistry is read-only.");
    }

    public void RegisterFunction(FunctionInfo functionInfo)
    {
        throw new ReadOnlyException("This FunctionRegistry is read-only.");
    }

    public bool ContainsFunctionName(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));

        return functions.ContainsKey(ConvertFunctionName(functionName));
    }

    private string ConvertFunctionName(string functionName)
    {
        return CaseSensitive ? functionName : functionName.ToLowerFast();
    }
}