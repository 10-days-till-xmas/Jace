using System;
using Jace.Execution;
using Xunit;

namespace Jace.Tests;

public sealed class ConstantRegistryTests
{
    [Fact]
    public void TestAddConstant()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0);

        var functionInfo = registry.GetConstantInfo("test");

        Assert.NotNull(functionInfo);
        Assert.Equal("test", functionInfo.ConstantName);
        Assert.Equal(42.0, functionInfo.Value);
    }

    [Fact]
    public void TestOverwritable()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0);
        registry.RegisterConstant("test", 26.3);
    }

    [Fact]
    public void TestNotOverwritable()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0, false);

        Assert.Throws<Exception>(() => registry.RegisterConstant("test", 26.3, false));
    }
}