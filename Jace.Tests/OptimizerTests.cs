using System;
using System.Collections.Generic;
using System.Globalization;
using Jace.Execution;
using Jace.Operations.BuiltIn;
using Jace.Tokenizer;
using Xunit;

namespace Jace.Tests;

public sealed class OptimizerTests
{
    [Fact]
    public void TestOptimizerIdempotentFunction()
    {
        var optimizer = new Optimizer(new Interpreter());

        var tokenReader = new TokenReader(CultureInfo.InvariantCulture);
        IList<Token> tokens = tokenReader.Read("test(var1, (2+3) * 500)");

        IFunctionRegistry functionRegistry = new FunctionRegistry(true);
        functionRegistry.RegisterFunction("test", (Func<double, double, double>)((a, b) => a + b));

        var astBuilder = new AstBuilder(true, functionRegistry);
        var operation = astBuilder.Build(tokens);

        var optimizedFunction = (Function)optimizer.Optimize(operation, functionRegistry, null!);

        Assert.IsType<FloatingPointConstant>(optimizedFunction.Arguments[1]);
    }

    [Fact]
    public void TestOptimizerNonIdempotentFunction()
    {
        var optimizer = new Optimizer(new Interpreter());

        var tokenReader = new TokenReader(CultureInfo.InvariantCulture);
        IList<Token> tokens = tokenReader.Read("test(500)");

        var functionRegistry = new FunctionRegistry(true);
        functionRegistry.RegisterFunction("test", (Func<double, double>)(a => a), false);

        var astBuilder = new AstBuilder(true, functionRegistry);
        var operation = astBuilder.Build(tokens);

        var optimizedFunction = optimizer.Optimize(operation, functionRegistry, null!);
        
        Assert.IsType<Function>(optimizedFunction);
        Assert.IsType<IntegerConstant>(((Function)optimizedFunction).Arguments[0]);
    }

    [Fact]
    public void TestOptimizerMultiplicationByZero()
    {
        var optimizer = new Optimizer(new Interpreter());

        var tokenReader = new TokenReader(CultureInfo.InvariantCulture);
        IList<Token> tokens = tokenReader.Read("var1 * 0.0");

        IFunctionRegistry functionRegistry = new FunctionRegistry(true);

        var astBuilder = new AstBuilder(true, functionRegistry);
        var operation = astBuilder.Build(tokens);

        var optimizedOperation = optimizer.Optimize(operation, functionRegistry, null!);
        
        Assert.IsType<FloatingPointConstant>(optimizedOperation);
        Assert.Equal(0.0, ((FloatingPointConstant)optimizedOperation).Value);
    }
}