namespace Jace.Execution;

public sealed class ConstantInfo(string constantName, double value, bool isReadOnly)
{
    public string ConstantName { get; } = constantName;

    public double Value { get; } = value;
    
    public bool IsReadOnly { get; } = isReadOnly;
}