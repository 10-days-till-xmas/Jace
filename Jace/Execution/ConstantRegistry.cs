using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Jace.Execution;

public class ConstantRegistry : IConstantRegistry
{
    // TODO: Implement more ConstantRegistry implementations,
    // e.g. ReadOnlyConstantRegistry, ConcurrentConstantRegistry, etc.
    private readonly Dictionary<string, ConstantInfo> constants; 

    public ConstantRegistry(IEnumerable<ConstantInfo> collection)
    {
        constants = collection.ToDictionary(c => c.ConstantName, c => c);
        CaseSensitive = true; // Default to case-sensitive for this constructor
    }

    public ConstantRegistry(bool caseSensitive)
    {
        CaseSensitive = caseSensitive;
        constants = new Dictionary<string, ConstantInfo>(caseSensitive
            ? StringComparer.Ordinal // Default case-sensitive comparer
            : StringComparer.OrdinalIgnoreCase); // Case-insensitive comparer (converts all keys to lower case)
    }

    [PublicAPI]
    public bool CaseSensitive { get; }

    /// <inheritdoc cref="IConstantRegistry.this[string]" />
    /// <exception cref="ArgumentNullException">Thrown when the constant name is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the specified constant name does not exist in the registry.</exception>
    [PublicAPI]
    public ConstantInfo this[string constantName]
    {
        get => constants[constantName];
        set => RegisterConstant(constantName, value.Value, value.IsReadOnly);
    }
    
    /// <inheritdoc cref="IConstantRegistry.TryGetConstantInfo"/>
    /// <exception cref="ArgumentNullException"><paramref name="constantName"/> is null</exception>
    public bool TryGetConstantInfo(string constantName, out ConstantInfo constantInfo)
        => constants.TryGetValue(constantName, out constantInfo);

    public bool Contains(string constantName)
    {
        return constants.ContainsKey(constantName);
    }

    public void RegisterConstant(string constantName, double value, bool isReadOnly = false)
    {
        if (TryGetConstantInfo(constantName, out var oldConstantInfo) && oldConstantInfo!.IsReadOnly)
            throw new InvalidOperationException(
                $"The constant \"{oldConstantInfo.ConstantName}\" cannot be overwritten.");
        constants[constantName] = new ConstantInfo(constantName, value, isReadOnly);
    }

    public void RegisterConstants(params ConstantInfo[] constantInfos)
    {
        // TODO: Add as an extension method to IConstantRegistry instead
        // extension methods could also allow for instantiating default ConstantRegistries
        if (constantInfos == null || constantInfos.Length == 0)
            return;
        foreach (var constantInfo in constantInfos)
            RegisterConstant(constantInfo);
    }

    public void RegisterConstant(ConstantInfo constantInfo)
    {
        RegisterConstant(constantInfo.ConstantName, constantInfo.Value, constantInfo.IsReadOnly);
    }
    
    public IEnumerator<ConstantInfo> GetEnumerator() => constants.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}