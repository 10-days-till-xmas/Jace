using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Jace.Execution;

[PublicAPI]
public class FunctionRegistry : RegistryBase<FunctionInfo>, IFunctionRegistry
{
    private const string SystemFuncPrefix = $"{nameof(System)}.{nameof(Func<object>)}";
    private const string DynamicFuncName = $"{nameof(Jace)}.{nameof(DynamicFunc<object, object>)}";

    private Dictionary<string, FunctionInfo> functions => items;

    public FunctionRegistry(IEnumerable<FunctionInfo> functions, bool caseSensitive) : base(caseSensitive)
    {
        foreach(var functionInfo in functions)
        {
            if (functionInfo == null)
                throw new ArgumentNullException(nameof(functions), "FunctionInfo cannot be null.");

            RegisterFunction(functionInfo);
        }
    }

    public FunctionRegistry(bool caseSensitive) : base(caseSensitive)
    {
    }

    public void RegisterFunction(FunctionInfo functionInfo) 
        => ValidateAndRegisterItem(functionInfo);

    public bool TryGetFunctionInfo(string functionName, out FunctionInfo? functionInfo) 
        => TryGetItem(functionName, out functionInfo);

    public void RegisterFunction(string functionName, Delegate function, bool isIdempotent = true, bool isReadOnly = false)
    {
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));

        if (function == null)
            throw new ArgumentNullException(nameof(function));

        var funcType = function.GetType();
        var isDynamicFunc = false;
        var numberOfParameters = -1;
            
        if (funcType.FullName?.StartsWith("System.Func") ?? false)
        {
            if (funcType.GenericTypeArguments.Any(genericArgument => genericArgument != typeof(double)))
                throw new ArgumentException("Only doubles are supported as function arguments.", nameof(function));

            numberOfParameters = function
                .GetMethodInfo()
                .GetParameters()
                .Count(p => p.ParameterType == typeof(double));
        }
        else if (funcType.FullName?.StartsWith(DynamicFuncName) ?? false)
        {
            isDynamicFunc = true;
        }
        else
            throw new ArgumentException($"Only System.Func and {DynamicFuncName} delegates are permitted.", nameof(function));

        if (functions.TryGetValue(functionName, out var existingFunctionInfo))
        {
            if (existingFunctionInfo.IsReadOnly)
                throw new InvalidOperationException($"The function \"{existingFunctionInfo.Name}\" cannot be overwritten.");
            if (existingFunctionInfo.IsDynamicFunc != isDynamicFunc)
                throw new InvalidOperationException("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
            if (existingFunctionInfo.NumberOfParameters != numberOfParameters)
                throw new InvalidOperationException("The number of parameters cannot be changed when overwriting a method.");
        }
        functions[functionName] = new FunctionInfo(functionName, numberOfParameters, isIdempotent, isReadOnly, isDynamicFunc, function);
    }

    public override FunctionInfo ValidateItem(FunctionInfo functionInfo)
    {
        var functionName = functionInfo.Name;
        var function = functionInfo.Function;
        
        if (string.IsNullOrEmpty(functionName) || function == null)
            throw new ArgumentNullException(nameof(functionInfo));
        
        AssertValidFunction(function);

        // Checks for existing function
        if (!functions.TryGetValue(functionName, out var existingFunctionInfo))
            return functionInfo;
        
        if (existingFunctionInfo.IsReadOnly)
            throw new Exception($"The function \"{functionName}\" cannot be overwritten.");
        if (existingFunctionInfo.NumberOfParameters != functionInfo.NumberOfParameters)
            throw new Exception("The number of parameters cannot be changed when overwriting a method.");
        if (existingFunctionInfo.IsDynamicFunc != functionInfo.IsDynamicFunc)
            throw new Exception("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.");
        return functionInfo;
    }

    /// <summary>
    /// Throws an exception if the function type is not valid.
    /// </summary>
    /// <param name="function"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void AssertValidFunction(Delegate function)
    {
        var functionType = function.GetType();
        if (functionType == typeof(DynamicFunc<object, object>))
            return; // DynamicFunc is always valid
        
        var funcTypeName = functionType.FullName;
        if (string.IsNullOrWhiteSpace(funcTypeName))
            throw new ArgumentException("Function type name cannot be null or empty.", nameof(functionType));

        var isSystemFunc = funcTypeName.StartsWith(SystemFuncPrefix);
        
        switch (isSystemFunc)
        {
            case true when functionType.GenericTypeArguments.Any(arg => arg != typeof(double)):
                throw new ArgumentException("Only doubles are supported as function arguments.", nameof(functionType));
            case false when !funcTypeName.StartsWith(DynamicFuncName):
                throw new ArgumentException($"Only {SystemFuncPrefix} and {DynamicFuncName} delegates are permitted.", nameof(functionType));
            // If it is a DynamicFunc, no further checks are needed.
        }
    }
}