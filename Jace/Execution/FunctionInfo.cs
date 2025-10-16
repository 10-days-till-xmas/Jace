using System;

namespace Jace.Execution;

public class FunctionInfo
{
    public FunctionInfo(string functionName, int numberOfParameters, bool isIdempotent, bool isOverWritable, bool isDynamicFunc, Delegate function)
    {
        FunctionName = functionName;
        NumberOfParameters = numberOfParameters;
        IsIdempotent = isIdempotent;
        IsOverWritable = isOverWritable;
        IsDynamicFunc = isDynamicFunc;
        Function = function;
    }

    public string FunctionName { get; private set; }

    public int NumberOfParameters { get; private set; }

    public bool IsOverWritable { get; set; }

    public bool IsIdempotent { get; set; }

    public bool IsDynamicFunc { get; private set; }

    public Delegate Function { get; private set; }
}