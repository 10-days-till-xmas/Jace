using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Yace.Interfaces;

namespace Yace.Execution;

public sealed class FunctionRegistry(bool caseSensitive, StringComparer? comparer = null) : IFunctionRegistry
{
    private const string DynamicFuncName = "Yace.DynamicFunc";
    public StringComparer Comparer { get; } = comparer ?? (caseSensitive
                                                               ? StringComparer.Ordinal
                                                               : StringComparer.OrdinalIgnoreCase);
    public bool CaseSensitive { get; } = caseSensitive;
    private readonly Dictionary<string, FunctionInfo> functions = new(comparer);

    public IEnumerator<FunctionInfo> GetEnumerator() => functions.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public FunctionInfo GetInfo(string functionName)
        => TryGetInfo(functionName, out var functionInfo)
               ? functionInfo
               : throw new KeyNotFoundException(functionName);

    public bool TryGetInfo(string functionName, [NotNullWhen(true)] out FunctionInfo? functionInfo)
    {
        return string.IsNullOrEmpty(functionName)
                   ? throw new ArgumentNullException(nameof(functionName))
                   : functions.TryGetValue(functionName, out functionInfo);
    }

    public void Register(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true)
    {
        if (string.IsNullOrWhiteSpace(functionName))
            throw new ArgumentNullException(nameof(functionName));

        if (function == null)
            throw new ArgumentNullException(nameof(function));

        var funcType = function.GetType();
        var isDynamicFunc = false;
        var numberOfParameters = -1;

        if (funcType.FullName!.StartsWith("System.Func"))
        {
            if (funcType.GenericTypeArguments.Any(genericArgument => genericArgument != typeof(double)))
                throw new ArgumentException("Only doubles are supported as function arguments.", nameof(function));

            numberOfParameters = function
                                .GetMethodInfo()
                                .GetParameters()
                                .Count(p => p.ParameterType == typeof(double));
        }
        else if (funcType.FullName.StartsWith(DynamicFuncName))
            isDynamicFunc = true;
        else
            throw new ArgumentException("Only System.Func and " + DynamicFuncName + " delegates are permitted.", nameof(function));

        if (functions.TryGetValue(functionName, out var funcInfo))
        {
            if (!funcInfo.IsOverWritable)
                throw new Exception($"The function \"{functionName}\" cannot be overwritten.");
            if (funcInfo.NumberOfParameters != numberOfParameters)
                throw new Exception("The number of parameters cannot be changed when overwriting a method.");
            if (funcInfo.IsDynamicFunc != isDynamicFunc)
                throw new Exception("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
        }

        var newFuncInfo = new FunctionInfo(functionName, numberOfParameters, isIdempotent, isOverWritable, isDynamicFunc, function);

        functions[functionName] = newFuncInfo;
    }

    public void Register(FunctionInfo functionInfo)
    {
        var functionName = functionInfo.Name;
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionInfo));

        if (functions.TryGetValue(functionName, out var oldFunctionInfo) && !oldFunctionInfo.IsOverWritable)
            throw new Exception($"The function \"{functionName}\" cannot be overwritten.");

        functions[functionName] = functionInfo;
    }

    public bool ContainsName(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));

        return functions.ContainsKey(functionName);
    }
}
