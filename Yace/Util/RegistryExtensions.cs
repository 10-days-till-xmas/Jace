using System.Collections.Generic;
using Yace.Execution;

namespace Yace.Util;

public static class RegistryExtensions
{
    public static ConstantRegistry ToRegistry(this IDictionary<string, double> source, bool isOverWritable = false)
    {
        var registry = new ConstantRegistry(false);
        foreach (var kvp in source)
            registry.Register(new ConstantInfo(kvp.Key, kvp.Value, isOverWritable));
        return registry;
    }
    public static ReadOnlyConstantRegistry ToReadOnlyRegistry(this IDictionary<string, double> source, bool isOverWritable = false)
    {
        return new ReadOnlyConstantRegistry(source.ToRegistry(isOverWritable));
    }
}
