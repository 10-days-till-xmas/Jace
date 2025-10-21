using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jace.Util;

namespace Jace.Execution;

public sealed class ConstantRegistry(bool caseSensitive) : IConstantRegistry
{
    private readonly Dictionary<string, ConstantInfo> constants = new();

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
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        constantName = ConvertConstantName(constantName);

        if (constants.TryGetValue(constantName, out var oldConstantInfo) && !oldConstantInfo.IsOverWritable)
            throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");

        constants[constantName] = new ConstantInfo(constantName, value, isOverWritable);
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        var constantName = constantInfo.ConstantName;
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        if (TryConvertConstantName(ref constantName))
            constantInfo = constantInfo with { ConstantName = constantName };

        if (constants.TryGetValue(constantName, out var oldConstantInfo) && !oldConstantInfo.IsOverWritable)
            throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");

        constants[constantName] = constantInfo;
    }

    private string ConvertConstantName(string constantName)
    {
        return caseSensitive
                   ? constantName
                   : constantName.ToLowerFast();
    }

    private bool TryConvertConstantName(ref string constantName)
    {
        if (caseSensitive) return false;
        constantName = constantName.ToLowerFast();
        return true;
    }
}