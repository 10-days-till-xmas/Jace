using System;
using Jace.Execution;
using Xunit;

// ReSharper disable ConvertToLocalFunction

namespace Jace.Tests;

public class FunctionRegistryTests
{
    [Fact]
    public void TestAddFunc2()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction = (a, b) => a * b;
        registry.RegisterFunction("test", testFunction);

        var functionInfo = registry.GetFunctionInfo("test");

        Assert.NotNull(functionInfo);
        Assert.Equal("test", functionInfo.FunctionName);
        Assert.Equal(2, functionInfo.NumberOfParameters);
        Assert.Equal(testFunction, functionInfo.Function);
    }

    [Fact]
    public void TestOverwritable()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction1 = (a, b) => a * b;
        Func<double, double, double> testFunction2 = (a, b) => a * b;
        registry.RegisterFunction("test", testFunction1);
        registry.RegisterFunction("test", testFunction2);
    }

    [Fact]
    public void TestNotOverwritable()
    {
        var registry = new FunctionRegistry(false);

        Func<double, double, double> testFunction1 = (a, b) => a * b;
        Func<double, double, double> testFunction2 = (a, b) => a * b;

        registry.RegisterFunction("test", testFunction1, true, false);

        Assert.Throws<Exception>(() => { registry.RegisterFunction("test", testFunction2, true, false); });
    }
}