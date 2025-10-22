using System;
using System.Collections.Generic;
using Jace.Execution;
using Jace.Util;
using Xunit;

namespace Jace.Tests;

public sealed class FuncAdapterTests
{
    [Fact]
    public void TestFuncAdapterWrap()
    {
        var parameters = new List<ParameterInfo>
        {
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.FloatingPoint }
        };

        double TestFunc(IDictionary<string, double> dictionary)
        {
            return dictionary["test1"] + dictionary["test2"];
        }

        var wrappedFunction = (Func<int, double, double>)FuncAdapter.Wrap(parameters, TestFunc);

        Assert.Equal(3.0, wrappedFunction(1, 2.0));
    }

    [Fact]
    public void TestFuncAdapterWrap_NoUnnecessaryClosures()
    {
        var parameters = new List<ParameterInfo>
        {
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.FloatingPoint }
        };
        static double TestFunc(IDictionary<string, double> dictionary) => dictionary["test1"] + dictionary["test2"];

        var wrappedFunction = (Func<int, double, double>)FuncAdapter.Wrap(parameters, TestFunc);

        Assert.Equal(3.0, wrappedFunction(1, 2.0));

        dynamic targetObj = wrappedFunction.Target!;
        var constants = (object[]) targetObj.Constants;
        Assert.Single(constants);
        Assert.Null((object[]) targetObj.Locals); // No locals should be defined
        // The constant should be the original function delegate (perhaps do IL manipulation instead to improve this?)
        Assert.IsType<Func<IDictionary<string, double>, double>>(constants[0]);
    }

    [Fact]
    public void TestFourArguments()
    {
        var parameters = new List<ParameterInfo>
        {
            new() { Name = "test1", DataType = DataType.Integer },
            new() { Name = "test2", DataType = DataType.Integer },
            new() { Name = "test3", DataType = DataType.Integer },
            new() { Name = "test4", DataType = DataType.Integer }
        };

        var wrappedFunction =
            (Func<int, int, int, int, double>)FuncAdapter.Wrap(parameters, dictionary => dictionary["test4"]);

        Assert.Equal(8.0, wrappedFunction(2, 4, 6, 8));
    }

}