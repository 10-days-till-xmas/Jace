using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jace.Execution;

namespace Jace.Tests.Mocks;
// I'm not sure how this'll be tested. TODO: Add tests for MockConstantRegistry
public class MockConstantRegistry(bool caseSensitive, Dictionary<string, ConstantInfo> constants) : IConstantRegistry
{
    public MockConstantRegistry(bool caseSensitive = false)
        : this(caseSensitive, new Dictionary<string, ConstantInfo>
        {
            {"pi", new ConstantInfo("pi", Math.PI, false)},
            {"e", new ConstantInfo("e", Math.E, false)}
        })
    {
    }

    public ConstantInfo GetConstantInfo(string constantName)
    {
        return constants[ConvertConstantName(constantName)];
    }

    public IEnumerator<ConstantInfo> GetEnumerator()
    {
        return constants.Select(c=> c.Value).GetEnumerator();
    }

    public bool IsConstantName(string constantName)
    {
        return constants.ContainsKey(ConvertConstantName(constantName));
    }

    public void RegisterConstant(string constantName, double value)
    {
        RegisterConstant(constantName, value, false);
    }

    public void RegisterConstant(string constantName, double value, bool isOverWritable)
    {
        if(string.IsNullOrEmpty(constantName))
            throw new ArgumentNullException(nameof(constantName));

        constantName = ConvertConstantName(constantName);

        var constantInfo = new ConstantInfo(constantName, value, isOverWritable);

        if (IsConstantName(constantName))
        {
            if (GetConstantInfo(constantName).IsOverWritable)
                constants[constantName] = constantInfo;
            else
                throw new Exception($"The constant \"{constantName}\" cannot be overwritten.");
        }
        else
        {
            constants.Add(constantName, constantInfo);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private string ConvertConstantName(string constantName)
    {
        return caseSensitive ? constantName : constantName.ToLower();
    }
}