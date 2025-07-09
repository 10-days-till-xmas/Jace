using System;
using JetBrains.Annotations;

namespace Jace.Execution;
// TODO: Implement more ConstantRegistry implementations,
// e.g. ReadOnlyConstantRegistry, ConcurrentConstantRegistry, etc.
[PublicAPI]
public class ConstantRegistry : RegistryBase<ConstantInfo>, IConstantRegistry
{
    public ConstantRegistry(params ConstantInfo[] constants) : base(constants) { }
    public ConstantRegistry(bool caseSensitive) : base(caseSensitive) { }

    /// <inheritdoc cref="RegistryBase{T}.TryGetItem"/>
    public bool TryGetConstantInfo(string constantName, out ConstantInfo? constantInfo)
        => TryGetItem(constantName, out constantInfo);

    public void RegisterConstant(string constantName, double value, bool isReadOnly = false) 
        => RegisterConstant(new ConstantInfo( constantName, value, isReadOnly));

    public void RegisterConstants(params ConstantInfo[] constantInfos)
    {
        // TODO: Add as an extension method to RegistryBase instead?
        // extension methods could also allow for instantiating default ConstantRegistries too
        if (constantInfos.Length == 0)
            return;
        foreach (var constantInfo in constantInfos)
            RegisterConstant(constantInfo);
    }

    public void RegisterConstant(ConstantInfo constantInfo)
        => ValidateAndRegisterItem(constantInfo);

    /// <inheritdoc cref="RegistryBase{T}.ValidateItem(T)"/>
    /// <exception cref="ArgumentException">Constant name is null or empty</exception>
    public override ConstantInfo ValidateItem(ConstantInfo item)
    {
        if (string.IsNullOrEmpty(item.Name))
        {
            throw new ArgumentException("Constant name cannot be null or empty.", nameof(item));
        }

        return item;
    }
}