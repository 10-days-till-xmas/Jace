using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yace.Execution;

public sealed class ConstantRegistry(bool caseSensitive, StringComparer? comparer = null) : IConstantRegistry
{
    public bool CaseSensitive { get; } = caseSensitive;
    public StringComparer Comparer { get; } = comparer ?? (caseSensitive
                                                               ? StringComparer.Ordinal
                                                               : StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ConstantInfo> constants = new();

    public ConstantRegistry(IConstantRegistry constantRegistry) : this(constantRegistry.CaseSensitive, constantRegistry.Comparer)
    {
        constants = new Dictionary<string, ConstantInfo>(constantRegistry.ToDictionary(ci => ci.Name, ci => ci, constantRegistry.Comparer));
    }

    public ConstantRegistry(ConstantRegistry constantRegistry) : this(constantRegistry.CaseSensitive, constantRegistry.Comparer)
    {
        constants = new Dictionary<string, ConstantInfo>(constantRegistry.constants);
    }

    public IEnumerator<ConstantInfo> GetEnumerator()
    {
        return constants.Values.GetEnumerator();
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
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        if (constants.TryGetValue(constantName, out var oldConstantInfo) && !oldConstantInfo.IsOverWritable)
            throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");

        constants[constantName] = new ConstantInfo(constantName, value, isOverWritable);
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        var constantName = constantInfo.Name;
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        if (constants.TryGetValue(constantName, out var oldConstantInfo) && !oldConstantInfo.IsOverWritable)
            throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");

        constants[constantName] = constantInfo;
    }
}
