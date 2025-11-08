using Yace.Execution;

namespace Yace.Util;

public static class RegistryExtensions
{
    public static FunctionRegistry Clone(this IFunctionRegistry source)
    {
        var clone = new FunctionRegistry(source.CaseSensitive);

        foreach (var function in source)
            clone.RegisterFunction(function);
        return clone;
    }
    public static ConstantRegistry Clone(this IConstantRegistry source)
    {
        var clone = new ConstantRegistry(source.CaseSensitive);

        foreach (var constant in source)
            clone.RegisterConstant(constant);
        return clone;
    }
}