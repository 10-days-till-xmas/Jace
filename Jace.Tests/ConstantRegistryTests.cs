using System;
using Jace.Execution;
using Xunit;

namespace Jace.Tests;

public class ConstantRegistryTests
{
    [Fact]
    public void TestAddConstant()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0);

        var functionInfo = registry["test"];

        Assert.NotNull(functionInfo);
        Assert.Equal("test", functionInfo.ConstantName);
        Assert.Equal(42.0, functionInfo.Value);
    }

    [Fact]
    public void TestOverwritable_CaseInsensitive()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0);
        registry.RegisterConstant("TeSt", 26.3);

        Assert.Equal(26.3, registry["test"].Value);
        Assert.Equal(26.3, registry["TeSt"].Value);
    }

    [Fact]
    public void TestOverwritable_CaseSensitive()
    {
        var registry = new ConstantRegistry(true);

        registry.RegisterConstant("test", 42.0);
        registry.RegisterConstant("TEST", 26.3);

        registry.RegisterConstant("test", 20.3);
        registry.RegisterConstant("TEST", -7.5);

        Assert.Equal(20.3, registry["test"].Value);
        Assert.Equal(-7.5, registry["TEST"].Value);
    }

    [Fact]
    public void TestNotOverwritable_CaseSensitive()
    {
        var registry = new ConstantRegistry(true);

        registry.RegisterConstant("test", 42.0, true);

        Assert.Throws<InvalidOperationException>(() => registry.RegisterConstant("test", 26.3, true));
    }

    [Fact]
    public void TestNotOverwritable_CaseInsensitive()
    {
        var registry = new ConstantRegistry(false);

        registry.RegisterConstant("test", 42.0, true);

        Assert.Throws<InvalidOperationException>(() => registry.RegisterConstant("TEST", 26.3, true));
    }
}