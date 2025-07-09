using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Jace.Execution;

/// <summary>
/// Represents a registry for managing items used in calculations.
/// </summary>
[PublicAPI]
public abstract class RegistryBase<T>(bool caseSensitive) : IEnumerable<T>
    where T : InfoItemBase 
{
    protected readonly Dictionary<string, T> items = new(caseSensitive
        ? StringComparer.Ordinal // Default case-sensitive comparer
        : StringComparer.OrdinalIgnoreCase); // Case-insensitive comparer (converts all keys to lower case)

    protected RegistryBase(params T[] items) : this(true)
    {
        this.items = items.ToDictionary(i => i.Name, i => i);
    }
    
    protected RegistryBase(IEnumerable<T> items, bool caseSensitive = true) : this(caseSensitive)
    {
        foreach (var item in items)
            ValidateAndRegisterItem(item);
    }
    
    public bool CaseSensitive { get; } = caseSensitive;

    /// <summary>
    /// Provides indexed access to items in the registry using their name as the key.
    /// </summary>
    /// <param name="itemName">The name of the item to retrieve or update in the registry.</param>
    /// <returns>The item associated with the specified name.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified item name doesn't exist in the registry.</exception>
    /// <exception cref="InvalidOperationException">Thrown when attempting to overwrite an existing read-only item.</exception>
    public T this[string itemName]
    {
        get => items[itemName];
        set => ValidateAndRegisterItem(value);
    }
    
    /// <summary>
    /// Attempts to retrieve an item from the registry by its name.
    /// </summary>
    /// <param name="itemName">The name of the item to retrieve.</param>
    /// <param name="item">
    /// When this method returns, contains the item associated with the specified name if found;
    /// otherwise, null.
    /// </param>
    /// <returns>true if the item with the specified name exists in the registry; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="itemName"/> is null.
    /// </exception>
    protected bool TryGetItem(string itemName, out T? item)
    {
        return items.TryGetValue(itemName, out item);
    }
    
    /// <summary>
    /// Determines whether the specified name corresponds to a registered constant in the registry.
    /// </summary>
    /// <param name="itemName">The name of the constant to check.</param>
    /// <returns>if the provided name matches a registered constant, otherwise false.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="itemName"/> is null</exception>
    public bool Contains(string itemName)
    {
        return items.ContainsKey(itemName);
    }
    
    /// <summary>
    /// Registers a new item in the registry or updates an existing one if it's overwritable.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when attempting to overwrite an existing read-only item</exception>
    protected void ValidateAndRegisterItem(T item)
    {
        if (TryGetItem(item.Name, out var existingItem) && existingItem!.IsReadOnly)
            throw new InvalidOperationException(
                $"The item \"{existingItem.Name}\" cannot be overwritten because it is read-only.");
        
        items[item.Name] = ValidateItem(item);
    }

    /// <summary>
    /// Checks if the item is valid for addition to the registry.
    /// </summary>
    /// <param name="item">The item to validate.</param>
    /// <returns>The item (if it is valid)</returns>
    public abstract T ValidateItem(T item);
    
    public IEnumerator<T> GetEnumerator() => items.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}