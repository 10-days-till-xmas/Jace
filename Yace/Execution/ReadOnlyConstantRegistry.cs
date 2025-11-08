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
    private readonly ReadOnlyDictionary<string, ConstantInfo> constants;

    public ReadOnlyConstantRegistry(IConstantRegistry innerRegistry)
    {
        CaseSensitive = innerRegistry.CaseSensitive;
        constants = new ReadOnlyDictionary<string, ConstantInfo>(innerRegistry.ToDictionary(c => c.ConstantName, c => c));
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
                   : constants.TryGetValue(ConvertConstantName(constantName), out constantInfo);
    }

    public bool ContainsConstantName(string constantName)
    {
        return string.IsNullOrEmpty(constantName)
                   ? throw new ArgumentNullException(nameof(constantName))
                   : constants.ContainsKey(ConvertConstantName(constantName));
    }

    public void RegisterConstant(string constantName, double value, bool isOverWritable = true)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        throw new ReadOnlyException("This ConstantRegistry is read-only.");
    }

    private string ConvertConstantName(string constantName)
    {
        return CaseSensitive
                   ? constantName
                   : constantName.ToLowerFast();
    }
}