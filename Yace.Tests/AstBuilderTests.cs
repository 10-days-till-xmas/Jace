using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yace.Execution;
using Yace.Operations;
using Yace.Tests.Mocks;
using Yace.Tokenizer;
using Xunit;

namespace Yace.Tests;

public sealed class AstBuilderTests
{
    [Fact]
    public void TestBuildFormula1()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: '(', TokenType.LeftBracket),
            new(value: 42),
            new(value: '+', TokenType.Operation),
            new(value: 8),
            new(value: ')', TokenType.RightBracket),
            new(value: '*', TokenType.Operation),
            new(value: 2)
        });

        var multiplication = (Multiplication)operation;
        var addition = (Addition)multiplication.Argument1;

        Assert.Equal(42, ((Constant<int>)addition.Argument1).Value);
        Assert.Equal(8, ((Constant<int>)addition.Argument2).Value);
        Assert.Equal(2, ((Constant<int>)multiplication.Argument2).Value);
    }

    [Fact]
    public void TestBuildFormula2()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 2),
            new(value: '+', TokenType.Operation),
            new(value: 8),
            new(value: '*', TokenType.Operation),
            new(value: 3)
        });

        var addition = (Addition)operation;
        var multiplication = (Multiplication)addition.Argument2;

        Assert.Equal(2, ((Constant<int>)addition.Argument1).Value);
        Assert.Equal(8, ((Constant<int>)multiplication.Argument1).Value);
        Assert.Equal(3, ((Constant<int>)multiplication.Argument2).Value);
    }

    [Fact]
    public void TestBuildFormula3()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 2),
            new(value: '*', TokenType.Operation),
            new(value: 8),
            new(value: '-', TokenType.Operation),
            new(value: 3)
        });

        var substraction = (Subtraction)operation;
        var multiplication = (Multiplication)substraction.Argument1;

        Assert.Equal(3, ((Constant<int>)substraction.Argument2).Value);
        Assert.Equal(2, ((Constant<int>)multiplication.Argument1).Value);
        Assert.Equal(8, ((Constant<int>)multiplication.Argument2).Value);
    }

    [Fact]
    public void TestDivision()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 10),
            new(value: '/', TokenType.Operation),
            new(value: 2)
        });

        var division = (Division)operation;

        Assert.Equal(new IntegerConstant(10), division.Argument1);
        Assert.Equal(new IntegerConstant(2), division.Argument2);
    }

    [Fact]
    public void TestMultiplication()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 10),
            new(value: '*', TokenType.Operation),
            new(value: 2.0)
        });

        var multiplication = (Multiplication)operation;

        Assert.Equal(new IntegerConstant(10), multiplication.Argument1);
        Assert.Equal(new FloatingPointConstant(2.0), multiplication.Argument2);
    }

    [Fact]
    public void TestExponentiation()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 2),
            new(value: '^', TokenType.Operation),
            new(value: 3)
        });

        var exponentiation = (Exponentiation)operation;

        Assert.Equal(new IntegerConstant(2), exponentiation.Argument1);
        Assert.Equal(new IntegerConstant(3), exponentiation.Argument2);
    }

    [Fact]
    public void TestModulo()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 2.7),
            new(value: '%', TokenType.Operation),
            new(value: 3)
        });

        var modulo = (Modulo)operation;

        Assert.Equal(new FloatingPointConstant(2.7), modulo.Argument1);
        Assert.Equal(new IntegerConstant(3), modulo.Argument2);
    }

    [Fact]
    public void TestVariable()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 10),
            new(value: '*', TokenType.Operation),
            new(value: "var1")
        });

        var multiplication = (Multiplication)operation;

        Assert.Equal(new IntegerConstant(10), multiplication.Argument1);
        Assert.Equal(new Variable("var1"), multiplication.Argument2);
    }

    [Fact]
    public void TestMultipleVariable()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: "var1"),
            new(value: '+', TokenType.Operation),
            new(value: 2),
            new(value: '*', TokenType.Operation),
            new(value: '(', TokenType.LeftBracket),
            new(value: 3),
            new(value: '*', TokenType.Operation),
            new(value: "age"),
            new(value: ')', TokenType.RightBracket)
        });

        var addition = (Addition)operation;
        var multiplication1 = (Multiplication)addition.Argument2;
        var multiplication2 = (Multiplication)multiplication1.Argument2;

        Assert.Equal(new Variable("var1"), addition.Argument1);
        Assert.Equal(new IntegerConstant(2), multiplication1.Argument1);
        Assert.Equal(new IntegerConstant(3), multiplication2.Argument1);
        Assert.Equal(new Variable("age"), multiplication2.Argument2);
    }

    [Fact]
    public void TestSinFunction1()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: "sin"),
            new(value: '(', TokenType.LeftBracket),
            new(value: 2),
            new(value: ')', TokenType.RightBracket)
        });

        var sineFunction = (Function)operation;

        Assert.Equal(new IntegerConstant(2), sineFunction.Arguments.Single());
    }

    [Fact]
    public void TestSinFunction2()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: "sin"),
            new(value: '(', TokenType.LeftBracket),
            new(value: 2),
            new(value: '+', TokenType.Operation),
            new(value: 3),
            new(value: ')', TokenType.RightBracket)
        });

        var sineFunction = (Function)operation;

        var addition = (Addition)sineFunction.Arguments.Single();
        Assert.Equal(new IntegerConstant(2), addition.Argument1);
        Assert.Equal(new IntegerConstant(3), addition.Argument2);
    }

    [Fact]
    public void TestSinFunction3()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: "sin"),
            new(value: '(', TokenType.LeftBracket),
            new(value: 2),
            new(value: '+', TokenType.Operation),
            new(value: 3),
            new(value: ')', TokenType.RightBracket),
            new(value: '*', TokenType.Operation),
            new(value: 4.9)
        });

        var multiplication = (Multiplication)operation;

        var sineFunction = (Function)multiplication.Argument1;

        var addition = (Addition)sineFunction.Arguments.Single();
        Assert.Equal(new IntegerConstant(2), addition.Argument1);
        Assert.Equal(new IntegerConstant(3), addition.Argument2);

        Assert.Equal(new FloatingPointConstant(4.9), multiplication.Argument2);
    }

    [Fact]
    public void TestUnaryMinus()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(value: 5.3),
            new(value: '*', TokenType.Operation),
            new(value: '_', TokenType.Operation),
            new(value: '(', TokenType.LeftBracket),
            new(value: 5),
            new(value: '+', TokenType.Operation),
            new(value: 42),
            new(value: ')', TokenType.RightBracket)
        });

        var multiplication = (Multiplication)operation;
        Assert.Equal(new FloatingPointConstant(5.3), multiplication.Argument1);

        var unaryMinus = (UnaryMinus)multiplication.Argument2;

        var addition = (Addition)unaryMinus.Argument;
        Assert.Equal(new IntegerConstant(5), addition.Argument1);
        Assert.Equal(new IntegerConstant(42), addition.Argument2);
    }

    [Fact]
    public void TestBuildInvalidFormula1()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);

        Assert.Throws<ParseException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: '(', TokenType.LeftBracket, startPosition: 0),
                new(value: 42, startPosition: 1),
                new(value: '+', TokenType.Operation, startPosition: 3),
                new(value: 8, startPosition: 4),
                new(value: ')', TokenType.RightBracket, startPosition: 5),
                new(value: '*', TokenType.Operation, startPosition: 6)
            });
        });
    }

    [Fact]
    public void TestBuildInvalidFormula2()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);

        Assert.Throws<ParseException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: 42, startPosition: 0),
                new(value: '+', TokenType.Operation, startPosition: 2),
                new(value: 8, startPosition: 3),
                new(value: ')', TokenType.RightBracket, startPosition: 4),
                new(value: '*', TokenType.Operation, startPosition: 5),
                new(value: 2, startPosition: 6)
            });
        });
    }

    [Fact]
    public void TestBuildInvalidFormula3()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);

        Assert.Throws<ParseException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: '(', TokenType.LeftBracket, startPosition: 0),
                new(value: 42, startPosition: 1),
                new(value: '+', TokenType.Operation, startPosition: 3),
                new(value: 8, startPosition: 4)
            });
        });
    }

    [Fact]
    public void TestBuildInvalidFormula4()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);

        Assert.Throws<ParseException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: 5, startPosition: 0),
                new(value: 42, startPosition: 1),
                new(value: '+', TokenType.Operation, startPosition: 3),
                new(value: 8, startPosition: 4)
            });
        });
    }

    [Fact]
    public void TestBuildInvalidFormula5()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);

        Assert.Throws<ParseException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: 42, startPosition: 0),
                new(value: '+', TokenType.Operation, startPosition: 2),
                new(value: 8, startPosition: 3),
                new(value: 5, startPosition: 4)
            });
        });
    }

    [Fact]
    public void TestBuildInvalidFormula6()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        object token = new Token(value: '+', TokenType.Operation, startPosition: 2);
        var f_TokenType = token.GetType().GetField("<TokenType>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!;

        f_TokenType.SetValue(token, (TokenType)10); // Invalid TokenType

        Assert.Equal(10, (int)((Token)token).TokenType);
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            builder.Build(new List<Token>
            {
                new(value: 42, startPosition: 0),
                (Token)token,
                new(value: 8, startPosition: 3),
                new(value: 5, startPosition: 4)
            });
        });
    }
}
