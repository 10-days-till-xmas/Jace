using System;
using System.Collections.Generic;
using Jace.Execution;
using Jace.Operations;
using Jace.Tests.Mocks;
using Xunit;

namespace Jace.Tests;

public class BasicInterpreterTests
{
    [Fact]
    public void TestBasicInterpreterSubstraction()
    {
        IFunctionRegistry functionRegistry = new MockFunctionRegistry();
        IConstantRegistry constantRegistry = new MockConstantRegistry();

        IExecutor executor = new Interpreter();
        var result = executor.Execute(new Subtraction(
            DataType.Integer,
            new IntegerConstant(6),
            new IntegerConstant(9)), functionRegistry, constantRegistry);

        Assert.Equal(-3.0, result);
    }

    [Fact]
    public void TestBasicInterpreter1()
    {
        IFunctionRegistry functionRegistry = new MockFunctionRegistry();
        IConstantRegistry constantRegistry = new MockConstantRegistry();

        IExecutor executor = new Interpreter();
        // 6 + (2 * 4)
        var result = executor.Execute(
            new Addition(
                DataType.Integer,
                new IntegerConstant(6),
                new Multiplication(
                    DataType.Integer,
                    new IntegerConstant(2),
                    new IntegerConstant(4))), functionRegistry, constantRegistry);

        Assert.Equal(14.0, result);
    }

    [Fact]
    public void TestBasicInterpreterWithVariables()
    {
        IFunctionRegistry functionRegistry = new MockFunctionRegistry();
        IConstantRegistry constantRegistry = new MockConstantRegistry();

        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "age", 4 }
        };

        IExecutor interpreter = new Interpreter();
        // var1 + 2 * (3 * age)
        var result = interpreter.Execute(
            new Addition(DataType.FloatingPoint,
                new Variable("var1"),
                new Multiplication(
                    DataType.FloatingPoint,
                    new IntegerConstant(2),
                    new Multiplication(
                        DataType.FloatingPoint,
                        new IntegerConstant(3),
                        new Variable("age")))), functionRegistry, constantRegistry, variables);

        Assert.Equal(26.0, result);
    }
}