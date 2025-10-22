﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Jace.Util;

namespace Jace.Execution;

public class FunctionRegistry : IFunctionRegistry
{
    private const string DynamicFuncName = "Jace.DynamicFunc";

    public bool CaseSensitive { get; }
    private readonly Dictionary<string, FunctionInfo> functions;

    public FunctionRegistry(bool caseSensitive)
    {
        CaseSensitive = caseSensitive;
        functions = new Dictionary<string, FunctionInfo>();
    }

    public IEnumerator<FunctionInfo> GetEnumerator()
    {
        return functions.Values.GetEnumerator();
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
        if (string.IsNullOrEmpty(functionName))
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

        functionName = ConvertFunctionName(functionName);

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

    public void RegisterFunction(FunctionInfo functionInfo)
    {
        var functionName = functionInfo.FunctionName;
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionInfo));
        if (TryConvertFunctionName(ref functionName))
            functionInfo = functionInfo with { FunctionName = functionName };

        if (functions.TryGetValue(functionName, out var oldFunctionInfo) && !oldFunctionInfo.IsOverWritable)
            throw new Exception($"The function \"{functionName}\" cannot be overwritten.");

        functions[functionName] = functionInfo;
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
    
    private bool TryConvertFunctionName(ref string functionName)
    {
        if (CaseSensitive) return false;
        functionName = functionName.ToLowerFast();
        return true;
    }
}