using System.Collections.Generic;
using Jace.Execution;
using Jace.Operations;
using Jace.Tests.Mocks;
using Xunit;

namespace Jace.Tests;

public class BasicInterpreterTests
{
    private static IConstantRegistry ConstantRegistry => MockConstantRegistry.GetPresetConstantRegistry();
    private static IFunctionRegistry FunctionRegistry => new MockFunctionRegistry();
    
    [Fact]
    public void TestBasicInterpreterSubstraction()
    {
        IExecutor executor = new Interpreter();
        var result = executor.Execute(new Subtraction(
            DataType.Integer,
            new IntegerConstant(6),
            new IntegerConstant(9)), FunctionRegistry, ConstantRegistry);

        Assert.Equal(-3.0, result);
    }

    [Fact]
    public void TestBasicInterpreter1()
    {
        IExecutor executor = new Interpreter();
        // 6 + (2 * 4)
        var result = executor.Execute(
            new Addition(
                DataType.Integer,
                new IntegerConstant(6),
                new Multiplication(
                    DataType.Integer,
                    new IntegerConstant(2),
                    new IntegerConstant(4))), FunctionRegistry, ConstantRegistry);

        Assert.Equal(14.0, result);
    }

    [Fact]
    public void TestBasicInterpreterWithVariables()
    {
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
                        new Variable("age")))), FunctionRegistry, ConstantRegistry, variables);

        Assert.Equal(26.0, result);
    }
}