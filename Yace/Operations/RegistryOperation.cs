using Yace.Execution;

namespace Yace.Operations;

public abstract class RegistryOperation<T> : Operation
    where T : RegistryItem
{
    public IRegistry<T> Registry { get; }
    protected RegistryOperation(
        IRegistry<T> registry,
        DataType dataType,
        bool dependsOnVariables,
        bool isIdempotent)
        : base(dataType, dependsOnVariables, isIdempotent)
    {
        Registry = registry;
    }
}
