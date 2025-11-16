using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yace.Execution;

namespace Yace.Tests.Mocks;
// I'm not sure how this'll be tested. TODO: Add tests for MockConstantRegistry
public sealed class MockConstantRegistry(bool caseSensitive, Dictionary<string, ConstantInfo> constants) : IConstantRegistry
{
    public StringComparer Comparer => StringComparer.OrdinalIgnoreCase;
    public MockConstantRegistry(bool caseSensitive = false)
        : this(caseSensitive, new Dictionary<string, ConstantInfo>
        {
            {"pi", new ConstantInfo("pi", Math.PI, false)},
            {"e", new ConstantInfo("e", Math.E, false)}
        })
    {
    }

    public bool CaseSensitive { get; } = caseSensitive;
    public ConstantInfo GetInfo(string constantName)
    {
        return constants[constantName];
    }
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    public bool TryGetInfo(string constantName, out ConstantInfo? constantInfo)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
    {
        return constants.TryGetValue(constantName, out constantInfo);
    }

    public IEnumerator<ConstantInfo> GetEnumerator()
    {
        return constants.Select(c=> c.Value).GetEnumerator();
    }

    public bool ContainsName(string constantName)
    {
        return constants.ContainsKey(constantName);
    }

    public void Register(ConstantInfo constantInfo)
    {
        Register(constantInfo.Name, constantInfo.Value, constantInfo.IsOverWritable);
    }

    public void Register(string constantName, double value, bool isOverWritable)
    {
        if(string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));
        

        var constantInfo = new ConstantInfo(constantName, value, isOverWritable);

        if (TryGetInfo(constantName, out var oldInfo))
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
    
}
