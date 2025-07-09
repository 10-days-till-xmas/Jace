using System;
using System.Linq;
using Jace.Execution;
using Jace.Util;
using Xunit;
#if XUNIT_V2
using Xunit.Abstractions;
#endif

// ReSharper disable ConvertToLocalFunction

namespace Jace.Tests;

public class FunctionRegistryTests(ITestOutputHelper testOutputHelper)
{
    private static Func<double, double, double> TestFunction1 => ( a, b) => a * b;
    private static Func<double, double, double> TestFunction2 => ( a, b) => a + b;
    private static Func<double, double, double> TestFunction3 => ( a, b) => a - b;
    private static Func<double, double, double> TestFunction4 => ( a, b) => a / b;
    
    [Fact]
    public void TestAddFunc()
    {
        var registry = new FunctionRegistry(false);
        
        registry.RegisterFunction("test", TestFunction1);

        var functionInfo = registry["test"];

        Assert.NotNull(functionInfo);
        Assert.Equal("test", functionInfo.Name);
        Assert.Equal(2, functionInfo.NumberOfParameters);
        Assert.Equal(TestFunction1, functionInfo.Function);
    }

    [Fact]
    public void TestOverwritable_CaseInsensitive()
    {
        var registry = new FunctionRegistry(caseSensitive: false);
        
        registry.RegisterFunction("test", TestFunction1);
        Assert.Equal(TestFunction1, registry["test"].Function);
        registry.RegisterFunction("test", TestFunction2);
        
        Assert.Equal(TestFunction2, registry["test"].Function);
        Assert.Equal(TestFunction2, registry["TeSt"].Function);
    }
    
    [Fact]
    public void TestOverwritable_CaseSensitive()
    {
        var registry = new FunctionRegistry(caseSensitive: true);


        registry.RegisterFunction("test", TestFunction1);
        registry.RegisterFunction("TEST", TestFunction2);
        
        registry.RegisterFunction("test", TestFunction3);
        registry.RegisterFunction("TEST", TestFunction4);
        
        Assert.Equal(TestFunction3, registry["test"].Function);
        Assert.Equal(TestFunction4, registry["TEST"].Function);
    }

    [Fact]
    public void TestNotOverwritable_CaseInsensitive()
    {
        var registry = new FunctionRegistry(caseSensitive: false);

        registry.RegisterFunction("test", TestFunction1, isReadOnly: true);

        var ex = Assert.Throws<InvalidOperationException>(() => registry.RegisterFunction("TEST", TestFunction2, isReadOnly: true));
        Assert.StartsWith("The function \"test\" cannot be overwritten.", ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
    
    [Fact]
    public void TestNotOverwritable_CaseSensitive()
    {
        var registry = new FunctionRegistry(caseSensitive: true);

        registry.RegisterFunction("test", TestFunction1, isReadOnly: true);
        
        var ex = Assert.Throws<InvalidOperationException>(() => registry.RegisterFunction("test", TestFunction2, isReadOnly: true));
        Assert.StartsWith("The function \"test\" cannot be overwritten.", ex.Message);
        testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
    
    [Fact]
    public void TestNotOverwritable_MismatchedParameterCount()
    {
        var registry = new FunctionRegistry(caseSensitive: false);
        
        var function1 = (Func<double, double, double>)((a, b) => a * b);
        var function2 = (Func<double, double, double, double>)((a, b, c) => a + b + c);
        
        registry.RegisterFunction("test", function1, isReadOnly: false);
        
        var ex = Assert.Throws<InvalidOperationException>(() => registry.RegisterFunction("test", function2, isReadOnly: true));
        Assert.StartsWith("The number of parameters cannot be changed when overwriting a method.", ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
    
    [Fact]
    public void TestNotOverwritable_FuncDynamicFuncMismatch()
    {
        var registry = new FunctionRegistry(caseSensitive: false);
        
        var function1 = (Func<double, double, double>)((a, b) => a * b);
        var function2 = (DynamicFunc<double, double>)((params double[] nums) => nums.Sum());
        
        registry.RegisterFunction("test", function1, isReadOnly: false);
        
        var ex = Assert.Throws<InvalidOperationException>(() => registry.RegisterFunction("test", function2, isReadOnly: true));
        Assert.StartsWith("A Func can only be overwritten by another Func and a DynamicFunc can only be overwritten by another DynamicFunc.", 
            ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }

    [Fact]
    public void TestAssertValidFunctionType_PassesIfFunc()
    {
        TestFunction1.AssertIsValidFunctionType();
    }
    
    [Fact]
    public void TestAssertValidFunctionType_PassesIfDynamicFunc()
    {
        DynamicFunc<double, double> dynamicFunc = (params double[] nums) => nums.Sum();
        dynamicFunc.AssertIsValidFunctionType();
    }
    
    [Fact]
    public void TestAssertValidFunctionType_ThrowsIfInvalidType()
    {
        var invalidFunc = (Action)(() => { });
        
        var ex = Assert.Throws<ArgumentException>(() => invalidFunc.AssertIsValidFunctionType());
        Assert.StartsWith("Only System.Func and Jace.DynamicFunc delegates are permitted.", ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
    
    [Fact]
    public void TestAssertValidFunctionType_ThrowsIfFuncWithInvalidGenericType()
    {
        var invalidFunc = (Func<string, string>)(s => s);
        
        var ex = Assert.Throws<ArgumentException>(() => invalidFunc.AssertIsValidFunctionType());
        Assert.StartsWith("Only doubles are supported as function arguments.", ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
    
    [Fact]
    public void TestAssertValidFunctionType_ThrowsIfDynamicFuncWithInvalidGenericType()
    {
        DynamicFunc<string, string> invalidFunc = (params string[] args) => string.Join(" ", args);
        
        var ex = Assert.Throws<ArgumentException>(() => invalidFunc.AssertIsValidFunctionType());
        Assert.StartsWith("Only doubles are supported as function arguments.", ex.Message);
        // testOutputHelper.WriteLine($"Exception message: {ex.Message}");
    }
}