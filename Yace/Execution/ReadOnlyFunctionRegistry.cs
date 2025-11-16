using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yace.Execution;

public sealed class ReadOnlyFunctionRegistry : IFunctionRegistry
{
    public static ReadOnlyFunctionRegistry Empty { get; } = new(new FunctionRegistry(true));
    public bool CaseSensitive { get; }

    public StringComparer Comparer { get; }
    private readonly ReadOnlyDictionary<string, FunctionInfo> functions;
    public IDictionary<string, FunctionInfo> Items => functions;
    public ReadOnlyFunctionRegistry(IFunctionRegistry innerFunctionRegistry)
    {
        CaseSensitive = innerFunctionRegistry.CaseSensitive;
        Comparer = innerFunctionRegistry.Comparer;
        functions = new ReadOnlyDictionary<string, FunctionInfo>(
            innerFunctionRegistry.ToDictionary(
                static f => f.Name,
                static f => f, Comparer));
    }
    public ReadOnlyFunctionRegistry(IFunctionRegistry? innerFunctionRegistry, bool caseSensitive, StringComparer? comparer = null)
    {
        CaseSensitive = caseSensitive;
        Comparer = comparer ?? (caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        functions = innerFunctionRegistry is null
                        #if !NET9_0_OR_GREATER
                        ? new ReadOnlyDictionary<string, FunctionInfo>(new Dictionary<string, FunctionInfo>(Comparer))
                        #else
                        ? ReadOnlyDictionary<string, FunctionInfo>.Empty
                        #endif
                        : new ReadOnlyDictionary<string, FunctionInfo>(innerFunctionRegistry.ToDictionary(
                            static f => f.Name,
                            static f => f, Comparer));
    }

    public IEnumerator<FunctionInfo> GetEnumerator()
    {
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        return functions.Values!.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public FunctionInfo GetInfo(string functionName)
    {
        return TryGetInfo(functionName, out var functionInfo)
                   ? functionInfo
                   : throw new KeyNotFoundException(functionName);
    }

    public bool TryGetInfo(string functionName, [NotNullWhen(true)] out FunctionInfo? functionInfo)
    {
        return string.IsNullOrEmpty(functionName)
                   ? throw new ArgumentNullException(nameof(functionName))
                   : functions.TryGetValue(functionName, out functionInfo);
    }

    public void Register(string functionName, Delegate function, bool isIdempotent = true, bool isOverWritable = true)
    {
        throw new ReadOnlyException("This FunctionRegistry is read-only.");
    }

    public void Register(FunctionInfo functionInfo)
    {
        throw new ReadOnlyException("This FunctionRegistry is read-only.");
    }

    public bool ContainsName(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));

        return functions.ContainsKey(functionName);
    }
}
