using System;
using System.Collections.Generic;
using Jace.Execution;
using Jace.Util;
using Xunit;

namespace Jace.Tests;

public class FuncAdapterTests
{
    [Fact]
    public void TestFuncAdapterWrap()
    {
        var adapter = new FuncAdapter();

        var parameters = new List<ParameterInfo>()
        {
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.FloatingPoint }
        };

        double TestFunc(IDictionary<string, double> dictionary) => dictionary["test1"] + dictionary["test2"];

        var wrappedFunction = (Func<int, double, double>)adapter.Wrap(parameters, TestFunc);

        Assert.Equal(3.0, wrappedFunction(1, 2.0));
    }

    [Fact]
    public void TestFuncAdapterWrapAndGC()
    {
        // TODO: Verify whether this actually tests anything
        var adapter = new FuncAdapter();
        
        var parameters = new List<ParameterInfo> { 
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.FloatingPoint }
        };

        double TestFunc(IDictionary<string, double> dictionary) => dictionary["test1"] + dictionary["test2"];

        var wrappedFunction = (Func<int, double, double>)adapter.Wrap(parameters, TestFunc);
        // ReSharper disable once RedundantAssignment
        adapter = null;
        
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        Assert.Equal(3.0, wrappedFunction(1, 2.0));
    }

    [Fact]
    public void TestFourArguments()
    {
        var adapter = new FuncAdapter();

        var parameters = new List<ParameterInfo> { 
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.Integer },
            new() { Name = "test3", DataType = DataType.Integer },
            new() { Name = "test4", DataType = DataType.Integer }
        };

        var wrappedFunction = (Func<int, int, int, int, double>)adapter.Wrap(parameters, dictionary => dictionary["test4"]);

        Assert.Equal(8.0, wrappedFunction(2, 4, 6, 8));
    }

    // Uncomment for debugging purposes
    //[TestMethod]
    //public void SaveToDisk()
    //{ 
    //    FuncAdapter adapter = new FuncAdapter();
    //    adapter.CreateDynamicModuleBuilder();
    //}
}