using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Yace.Util;

namespace Yace.Execution;

public sealed class ReadOnlyConstantRegistry : IConstantRegistry
{
    public bool CaseSensitive { get; }
    public StringComparer Comparer { get; }
    private readonly ReadOnlyDictionary<string, ConstantInfo> constants;

    public ReadOnlyConstantRegistry(IConstantRegistry innerRegistry)
    {
        CaseSensitive = innerRegistry.CaseSensitive;
        Comparer = innerRegistry.Comparer;
        constants = new ReadOnlyDictionary<string, ConstantInfo>(
            innerRegistry.ToDictionary(
                static c => c.ConstantName,
                static c => c,
                Comparer));
    }

    public IEnumerator<ConstantInfo> GetEnumerator()
    {
        // ReSharper disable once RedundantSuppressNullableWarningExpression
        return constants.Values!.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ConstantInfo GetConstantInfo(string constantName)
    {
        return TryGetConstantInfo(constantName, out var constantInfo)
                   ? constantInfo
                   : throw new KeyNotFoundException(constantName);
    }

    public bool TryGetConstantInfo(string constantName, [NotNullWhen(true)] out ConstantInfo? constantInfo)
    {
        return string.IsNullOrEmpty(constantName)
                   ? throw new ArgumentNullException(nameof(constantName))
                   : constants.TryGetValue(constantName, out constantInfo);
    }

    public bool ContainsConstantName(string constantName)
    {
        return string.IsNullOrEmpty(constantName)
                   ? throw new ArgumentNullException(nameof(constantName))
                   : constants.ContainsKey(constantName);
    }

    public void RegisterConstant(string constantName, double value, bool isOverWritable = true)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }
}
