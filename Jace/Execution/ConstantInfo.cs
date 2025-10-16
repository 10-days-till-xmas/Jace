namespace Jace.Execution;

public class ConstantInfo
{
    public ConstantInfo(string constantName, double value, bool isOverWritable)
    {
        ConstantName = constantName;
        Value = value;
        IsOverWritable = isOverWritable;
    }

    public string ConstantName { get; private set; }

    public double Value { get; private set; }

    public bool IsOverWritable { get; set; }
}