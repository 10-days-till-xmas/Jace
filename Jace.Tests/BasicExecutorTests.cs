using System;
using System.Collections.Generic;
using Jace.Execution;
using Jace.Operations.BuiltIn;
using Jace.Tests.Mocks;
using Jace.Tests.Mocks.Extensibility;
using JetBrains.Annotations;
using Xunit;

namespace Jace.Tests;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors, 
    Reason = "This class is used as a base class for tests.")]
public abstract class BasicExecutorTests<T> where T : IExecutor
{
    private static IConstantRegistry ConstantRegistry => MockConstantRegistry.GetPresetConstantRegistry();
    private static IFunctionRegistry FunctionRegistry => new MockFunctionRegistry();

    private static IExecutor Executor => Activator.CreateInstance(typeof(T), [false]) as IExecutor
                                         ?? throw new InvalidOperationException(
                                             "Could not create instance of executor.");
    
    [Fact]
    public void TestBasicExecutorSubstraction()
    {
        var executor = Executor;
        var result = executor.Execute(new Subtraction(
            DataType.Integer,
            new IntegerConstant(6),
            new IntegerConstant(9)), FunctionRegistry, ConstantRegistry);

        Assert.Equal(-3.0, result);
    }

    [Fact]
    public void TestBasicExecutor()
    {
        // 6 + (2 * 4)
        var result = Executor.Execute(
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
    public void TestBasicExecutorWithVariables()
    {
        var variables = new Dictionary<string, double>
        {
            { "var1", 2 },
            { "age", 4 }
        };
        
        // var1 + 2 * (3 * age)
        var result = Executor.Execute(
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

    [Fact]
    public void TestBasicExecutorWithOperationExtension_Unary()
    {
        var result = Executor.Execute(
            new Add5(DataType.FloatingPoint,
                new FloatingPointConstant(10.0)), 
            FunctionRegistry, 
            ConstantRegistry);
        Assert.Equal(15.0, result);
    }
    [Fact]
    public void TestBasicExecutorWithOperationExtension_Binary()
    {
        var result = Executor.Execute(
            new CalculateHypotenuse(DataType.FloatingPoint,
                new FloatingPointConstant(3.0),
                new FloatingPointConstant(4.0)
                ), 
            FunctionRegistry, 
            ConstantRegistry);
        Assert.Equal(5.0, result);
    }
}
public sealed class BasicInterpreterTests : BasicExecutorTests<Interpreter>;
public class BasicDynamicCompilerTests : BasicExecutorTests<DynamicCompiler>;