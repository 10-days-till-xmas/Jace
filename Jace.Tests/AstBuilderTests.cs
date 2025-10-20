using System.Collections.Generic;
using System.Linq;
using Jace.Execution;
using Jace.Operations;
using Jace.Tests.Mocks;
using Jace.Tokenizer;
using Xunit;

namespace Jace.Tests;

public class AstBuilderTests
{
    [Fact]
    public void TestBuildFormula1()
    {
        IFunctionRegistry registry = new MockFunctionRegistry();

        var builder = new AstBuilder(registry, false);
        var operation = builder.Build(new List<Token>
        {
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 42, TokenType.Integer),
            new(Value: '+', TokenType.Operation),
            new(Value: 8, TokenType.Integer),
            new(Value: ')', TokenType.RightBracket),
            new(Value: '*', TokenType.Operation),
            new(Value: 2, TokenType.Integer)
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
            new(Value: 2, TokenType.Integer),
            new(Value: '+', TokenType.Operation),
            new(Value: 8, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: 3, TokenType.Integer)
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
            new(Value: 2, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: 8, TokenType.Integer),
            new(Value: '-', TokenType.Operation),
            new(Value: 3, TokenType.Integer)
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
            new(Value: 10, TokenType.Integer),
            new(Value: '/', TokenType.Operation),
            new(Value: 2, TokenType.Integer)
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
            new(Value: 10, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: 2.0, TokenType.FloatingPoint)
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
            new(Value: 2, TokenType.Integer),
            new(Value: '^', TokenType.Operation),
            new(Value: 3, TokenType.Integer)
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
            new(Value: 2.7, TokenType.FloatingPoint),
            new(Value: '%', TokenType.Operation),
            new(Value: 3, TokenType.Integer)
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
            new(Value: 10, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: "var1", TokenType.Text)
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
            new(Value: "var1", TokenType.Text),
            new(Value: '+', TokenType.Operation),
            new(Value: 2, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 3, TokenType.Integer),
            new(Value: '*', TokenType.Operation),
            new(Value: "age", TokenType.Text),
            new(Value: ')', TokenType.RightBracket)
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
            new(Value: "sin", TokenType.Text),
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 2, TokenType.Integer),
            new(Value: ')', TokenType.RightBracket)
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
            new(Value: "sin", TokenType.Text),
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 2, TokenType.Integer),
            new(Value: '+', TokenType.Operation),
            new(Value: 3, TokenType.Integer),
            new(Value: ')', TokenType.RightBracket)
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
            new(Value: "sin", TokenType.Text),
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 2, TokenType.Integer),
            new(Value: '+', TokenType.Operation),
            new(Value: 3, TokenType.Integer),
            new(Value: ')', TokenType.RightBracket),
            new(Value: '*', TokenType.Operation),
            new(Value: 4.9, TokenType.FloatingPoint)
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
            new(Value: 5.3, TokenType.FloatingPoint),
            new(Value: '*', TokenType.Operation),
            new(Value: '_', TokenType.Operation),
            new(Value: '(', TokenType.LeftBracket),
            new(Value: 5, TokenType.Integer),
            new(Value: '+', TokenType.Operation),
            new(Value: 42, TokenType.Integer),
            new(Value: ')', TokenType.RightBracket)
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
                new(Value: '(', TokenType.LeftBracket, StartPosition: 0),
                new(Value: 42, TokenType.Integer, StartPosition: 1),
                new(Value: '+', TokenType.Operation, StartPosition: 3),
                new(Value: 8, TokenType.Integer, StartPosition: 4),
                new(Value: ')', TokenType.RightBracket, StartPosition: 5),
                new(Value: '*', TokenType.Operation, StartPosition: 6)
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
                new(Value: 42, TokenType.Integer, StartPosition: 0),
                new(Value: '+', TokenType.Operation, StartPosition: 2),
                new(Value: 8, TokenType.Integer, StartPosition: 3),
                new(Value: ')', TokenType.RightBracket, StartPosition: 4),
                new(Value: '*', TokenType.Operation, StartPosition: 5),
                new(Value: 2, TokenType.Integer, StartPosition: 6)
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
                new(Value: '(', TokenType.LeftBracket, StartPosition: 0),
                new(Value: 42, TokenType.Integer, StartPosition: 1),
                new(Value: '+', TokenType.Operation, StartPosition: 3),
                new(Value: 8, TokenType.Integer, StartPosition: 4)
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
                new(Value: 5, TokenType.Integer, StartPosition: 0),
                new(Value: 42, TokenType.Integer, StartPosition: 1),
                new(Value: '+', TokenType.Operation, StartPosition: 3),
                new(Value: 8, TokenType.Integer, StartPosition: 4)
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
                new(Value: 42, TokenType.Integer, StartPosition: 0),
                new(Value: '+', TokenType.Operation, StartPosition: 2),
                new(Value: 8, TokenType.Integer, StartPosition: 3),
                new(Value: 5, TokenType.Integer, StartPosition: 4)
            });
        });
    }
}