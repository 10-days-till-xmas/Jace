using System;
using System.Collections;
using System.Collections.Generic;
using Jace.Util;

namespace Jace.Execution;

public class ConstantRegistry(bool caseSensitive) : IConstantRegistry
{
    private readonly Dictionary<string, ConstantInfo> constants = [];

    public IEnumerator<ConstantInfo> GetEnumerator() => constants.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public ConstantInfo GetConstantInfo(string constantName)
    {
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));
            
        return constants[ConvertConstantName(constantName)];
    }

    public bool IsConstantName(string constantName)
    {
        if (string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        return constants.ContainsKey(ConvertConstantName(constantName));
    }

    public void RegisterConstant(string constantName, double value, bool isReadOnly = false)
    {
        if(string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        constantName = ConvertConstantName(constantName);
        
        if (constants.TryGetValue(constantName, out var constantInfo) && constantInfo.IsReadOnly)
            throw new InvalidOperationException($"The constant \"{constantName}\" cannot be overwritten.");
        constants[constantName] = new ConstantInfo(constantName, value, isReadOnly);
    }

    private string ConvertConstantName(string constantName)
    {
        return caseSensitive ? constantName : constantName.ToLowerFast();
    }
}