using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Jace.Execution;

namespace Jace.Tests.Mocks;
// I'm not sure how this'll be tested. TODO: Add tests for MockConstantRegistry
public sealed class MockConstantRegistry(bool caseSensitive, Dictionary<string, ConstantInfo> constants) : IConstantRegistry
{
    public MockConstantRegistry(bool caseSensitive = false)
        : this(caseSensitive, new Dictionary<string, ConstantInfo>
        {
            {"pi", new ConstantInfo("pi", Math.PI, false)},
            {"e", new ConstantInfo("e", Math.E, false)}
        })
    {
    }

    public bool CaseSensitive { get; } = caseSensitive;
    public ConstantInfo GetConstantInfo(string constantName)
    {
        return constants[ConvertConstantName(constantName)];
    }
    public bool TryGetConstantInfo(string constantName, out ConstantInfo? constantInfo)
    {
        return constants.TryGetValue(ConvertConstantName(constantName), out constantInfo);
    }

    public IEnumerator<ConstantInfo> GetEnumerator()
    {
        return constants.Select(c=> c.Value).GetEnumerator();
    }

    public bool ContainsConstantName(string constantName)
    {
        return constants.ContainsKey(ConvertConstantName(constantName));
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        RegisterConstant(constantInfo.ConstantName, constantInfo.Value, constantInfo.IsOverWritable);
    }

    public void RegisterConstant(string constantName, double value, bool isOverWritable)
    {
        if(string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        constantName = ConvertConstantName(constantName);

        var constantInfo = new ConstantInfo(constantName, value, isOverWritable);

        if (TryGetConstantInfo(constantName, out var oldInfo))
            if (oldInfo!.IsOverWritable)
                constants[constantName] = constantInfo;
            else
                throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");
        else
            constants.Add(constantName, constantInfo);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private string ConvertConstantName(string constantName)
    {
        return CaseSensitive ? constantName : constantName.ToLower();
    }
}