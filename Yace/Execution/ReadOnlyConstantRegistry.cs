using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Yace.Interfaces;

namespace Yace.Execution;

public sealed class ReadOnlyConstantRegistry : IConstantRegistry
{
    public static ReadOnlyConstantRegistry Empty { get; } = new(new ConstantRegistry(true));
    public bool CaseSensitive { get; }
    public StringComparer Comparer { get; }
    private readonly ReadOnlyDictionary<string, ConstantInfo> constants;

    public ReadOnlyConstantRegistry(IConstantRegistry innerRegistry)
    {
        CaseSensitive = innerRegistry.CaseSensitive;
        Comparer = innerRegistry.Comparer;
        constants = new ReadOnlyDictionary<string, ConstantInfo>(
            innerRegistry.ToDictionary(
                static c => c.Name,
                static c => c,
                Comparer));
    }
    public ReadOnlyConstantRegistry(IConstantRegistry? innerRegistry, bool caseSensitive, StringComparer? comparer = null)
    {
        CaseSensitive = caseSensitive;
        Comparer = comparer ?? (caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
        constants = innerRegistry is null
                        #if !NET9_0_OR_GREATER
                        ? new ReadOnlyDictionary<string, ConstantInfo>(new Dictionary<string, ConstantInfo>(Comparer))
                        #else
                        ? ReadOnlyDictionary<string, ConstantInfo>.Empty
                        #endif
                        : new ReadOnlyDictionary<string, ConstantInfo>(innerRegistry.ToDictionary(
                            static f => f.Name,
                            static f => f, Comparer));
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

    public ConstantInfo GetInfo(string constantName)
    {
        return TryGetInfo(constantName, out var constantInfo)
                   ? constantInfo
                   : throw new KeyNotFoundException(constantName);
    }

    public bool TryGetInfo(string constantName, [NotNullWhen(true)] out ConstantInfo? constantInfo)
    {
        return string.IsNullOrEmpty(constantName)
                   ? throw new ArgumentNullException(nameof(constantName))
                   : constants.TryGetValue(constantName, out constantInfo);
    }

    public bool ContainsName(string constantName)
    {
        return string.IsNullOrEmpty(constantName)
                   ? throw new ArgumentNullException(nameof(constantName))
                   : constants.ContainsKey(constantName);
    }

    public void Register(string constantName, double value, bool isOverWritable = true)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }

    public void Register(ConstantInfo constantInfo)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }
}
