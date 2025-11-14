using System;
using Yace.Execution;
using Xunit;

// ReSharper disable ConvertToLocalFunction

namespace Yace.Tests;

public sealed class FunctionRegistryTests
{
    [Fact]
    public void TestAddFunc2()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction = (a, b) => a * b;
        registry.Register("test", testFunction);

        var functionInfo = registry.GetInfo("test");

        Assert.NotNull(functionInfo);
        Assert.Equal("test", functionInfo.Name);
        Assert.Equal(2, functionInfo.NumberOfParameters);
        Assert.Equal(testFunction, functionInfo.Function);
    }

    [Fact]
    public void TestOverwritable()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction1 = (a, b) => a * b;
        Func<double, double, double> testFunction2 = (a, b) => a * b;
        registry.Register("test", testFunction1);
        registry.Register("test", testFunction2);
    }

    [Fact]
    public void TestNotOverwritable()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction1 = (a, b) => a * b;
        Func<double, double, double> testFunction2 = (a, b) => a * b;

        registry.Register("test", testFunction1, true, false);

        Assert.Throws<Exception>(() => { registry.Register("test", testFunction2, true, false); });
    }
}