using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yace.Execution;

namespace Yace.Interfaces;

public interface IRegistry<T> : IEnumerable<T>, IUsesText
    where T : RegistryItem
{
    StringComparer Comparer { get; }
    T GetInfo(string name);
    bool TryGetInfo(string name, [NotNullWhen(true)] out T? info);
    bool ContainsName(string name);
    void Register(T item);
}
