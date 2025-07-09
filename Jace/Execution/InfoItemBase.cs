using System;

namespace Jace.Execution;

public abstract record InfoItemBase
{
    protected InfoItemBase(string name, bool isReadOnly)
    {
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

        Name = name;
        IsReadOnly = isReadOnly;
    }

    public string Name { get; }
    public bool IsReadOnly { get; }
}