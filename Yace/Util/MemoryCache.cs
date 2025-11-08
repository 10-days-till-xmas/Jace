using System;
using System.Collections.Generic;

namespace Yace.Util;

/// <summary>
/// An in-memory-based cache to store objects. The implementation is thread-safe and supports
/// the multiple platforms supported by Yace (.NET, WinRT, WP7 and WP8).
/// </summary>
/// <typeparam name="TKey">The type of the keys.</typeparam>
/// <typeparam name="TValue">The type of the values.</typeparam>
public sealed class MemoryCache<TKey, TValue> where TKey : notnull
{
    private readonly uint maximumSize;
    private readonly uint reductionSize;

    private readonly Dictionary<TKey, LinkedListNode<TValue>> dictionary;
    private readonly LinkedList<TValue> lruList;
    private readonly object _lock = new();


    /// <summary>
    /// Create a new instance of the <see cref="MemoryCache{TKey,TValue}"/>.
    /// </summary>
    /// <param name="maximumSize">The maximum allowed number of items in the cache.</param>
    /// <param name="reductionSize">The number of items to be deleted per cleanup of the cache.</param>
    public MemoryCache(uint maximumSize, uint reductionSize)
    {
        if (maximumSize < 1)
            throw new ArgumentOutOfRangeException(nameof(maximumSize),
                "The maximum allowed number of items in the cache must be at least one.");

        if (reductionSize < 1)
            throw new ArgumentOutOfRangeException(nameof(reductionSize),
                "The cache reduction size must be at least one.");

        this.maximumSize = maximumSize;
        this.reductionSize = reductionSize;

        dictionary = new Dictionary<TKey, LinkedListNode<TValue>>();
        lruList = [];
    }

    public MemoryCache(int maximumSize, int reductionSize)
        : this(maximumSize >= 1
                   ? (uint)maximumSize
                   : throw new ArgumentOutOfRangeException(nameof(maximumSize),
                         "The maximum allowed number of items in the cache must be at least one."),
            reductionSize >= 1
                ? (uint)reductionSize
                : throw new ArgumentOutOfRangeException(nameof(reductionSize),
                      "The cache reduction size must be at least one."))

    { }

    /// <summary>
    /// Get the value in the cache for the given key.
    /// </summary>
    /// <param name="key">The key to lookup in the cache.</param>
    /// <returns>The value for the given key.</returns>
    public TValue this[TKey key]
    {
        get
        {
            lock (_lock)
            {
                var cacheNode = dictionary[key];
                lruList.Remove(cacheNode);
                lruList.AddFirst(cacheNode);
                return cacheNode.Value;
            }
        }
    }

    /// <summary>
    /// Gets the number of items in the cache.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock) return dictionary.Count;
        }
    }

    /// <summary>
    /// Returns true if an item with the given key is present in the cache.
    /// </summary>
    /// <param name="key">The key to lookup in the cache.</param>
    /// <returns>True if an item is present in the cache for the given key.</returns>
    public bool ContainsKey(TKey key)
    {
        lock (_lock) return dictionary.ContainsKey(key);
    }
    public bool TryGetValue(TKey key, out TValue? value)
    {
        lock (_lock)
            if (dictionary.TryGetValue(key, out var node))
            {
                lruList.Remove(node);
                lruList.AddFirst(node);
                value = node.Value;
                return true;
            }
        value = default;
        return false;
    }

    /// <summary>
    /// If for a given key an item is present in the cache, this method will return
    /// the value for the given key. If no item is present in the cache for the given
    /// key, the valueFactory is executed to produce the value. This value is stored in
    /// the cache and returned to the caller.
    /// </summary>
    /// <param name="key">The key to lookup in the cache.</param>
    /// <param name="valueFactory">The factory to produce the value matching with the key.</param>
    /// <returns>The value for the given key.</returns>
    public TValue? GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        if (valueFactory == null)
            throw new ArgumentNullException(nameof(valueFactory));

        lock (_lock)
        {
            if (TryGetValue(key, out var val))
                return val;
            TrimIfFull();
            var value = valueFactory(key);
            var newNode = new LinkedListNode<TValue>(value);
            lruList.AddFirst(newNode);
            dictionary[key] = newNode;
            return value;
        }
    }

    /// <summary>
    /// Ensure that the cache has room for an additional item.
    /// If there is not enough room anymore, force a removal of oldest
    /// accessed items in the cache.
    /// </summary>
    private void TrimIfFull()
    {
        if (dictionary.Count < maximumSize) return;
        lock (_lock)
            for (var i = 0; i < reductionSize; i++)
            {
                var lruNode = lruList.Last;
                if (lruNode == null) break;
                lruList.RemoveLast();
                foreach (var kvp in dictionary)
                {
                    if (kvp.Value != lruNode) continue;
                    var keyToRemove = kvp.Key;
                    dictionary.Remove(keyToRemove);
                    break;
                }
            }
    }

    public bool Remove(TKey key)
    {
        lock (_lock)
        {
            if (!dictionary.TryGetValue(key, out var node)) return false;
            lruList.Remove(node);
            dictionary.Remove(key);
            return true;
        }
    }
}