namespace Yace.Execution;

public sealed record ConstantInfo(
    string Name,
    double Value,
    bool IsOverWritable) : RegistryItem(Name, IsOverWritable);
