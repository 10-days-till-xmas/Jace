using System.Globalization;
using Jace.Tokenizer;
using Xunit;
// ReSharper disable RedundantArgumentDefaultValue

namespace Jace.Tests;

public sealed class TokenReaderTests
{
    [Fact]
    public void TestTokenReader01()
    {
        var reader = new TokenReader();
        var tokens = reader.Read(formula: "42+31");

        Assert.Equal(3, tokens.Count);
        
        Assert.Equal(new Token(42, TokenType.Integer, StartPosition: 0, Length: 2), tokens[index: 0]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 2, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(31, TokenType.Integer, StartPosition: 3, Length: 2), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader02()
    {
        var reader = new TokenReader();
        var tokens = reader.Read(formula: "(42+31)");

        Assert.Equal(5, tokens.Count);
        
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token(42, TokenType.Integer, StartPosition: 1, Length: 2), tokens[index: 1]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 3, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(31, TokenType.Integer, StartPosition: 4, Length: 2), tokens[index: 3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, StartPosition: 6, Length: 1), tokens[index: 4]);
    }

    [Fact]
    public void TestTokenReader03()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "(42+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token(42, TokenType.Integer, StartPosition: 1, Length: 2), tokens[index: 1]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 3, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, StartPosition: 4, Length: 4), tokens[index: 3]);
    }

    [Fact]
    public void TestTokenReader04()
    {
        var reader = new TokenReader();
        var tokens = reader.Read(formula: "(42+ 8) *2");

        Assert.Equal(7, tokens.Count);
        
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token(42, TokenType.Integer, StartPosition: 1, Length: 2), tokens[index: 1]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 3, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(8, TokenType.Integer, StartPosition: 5, Length: 1), tokens[index: 3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, StartPosition: 6, Length: 1), tokens[index: 4]);
        Assert.Equal(new Token('*', TokenType.Operation, StartPosition: 8, Length: 1), tokens[index: 5]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 9, Length: 1), tokens[index: 6]);
    }

    [Fact]
    public void TestTokenReader05()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "(42.87+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token(42.87, TokenType.FloatingPoint, StartPosition: 1, Length: 5), tokens[index: 1]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 6, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, StartPosition: 7, Length: 4), tokens[index: 3]);
    }

    [Fact]
    public void TestTokenReader06()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "(var+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token("var", TokenType.Text, StartPosition: 1, Length: 3), tokens[index: 1]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 4, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, StartPosition: 5, Length: 4), tokens[index: 3]);
    }

    [Fact]
    public void TestTokenReader07()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);

        var tokens = reader.Read(formula: "varb");

        Assert.Single(collection: tokens);

        Assert.Equal(new Token("varb", TokenType.Text, StartPosition: 0, Length: 4), tokens[index: 0]);
    }

    [Fact]
    public void TestTokenReader08()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "varb(");

        Assert.Equal(2, tokens.Count);

        Assert.Equal(new Token("varb", TokenType.Text, StartPosition: 0, Length: 4), tokens[index: 0]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 4, Length: 1), tokens[index: 1]);
    }

    [Fact]
    public void TestTokenReader09()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "+varb(");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token("varb", TokenType.Text, StartPosition: 1, Length: 4), tokens[index: 1]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 5, Length: 1), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader10()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "var1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var1", TokenType.Text, StartPosition: 0, Length: 4), tokens[index: 0]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 4, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 5, Length: 1), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader11()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "5.1%2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(5.1, TokenType.FloatingPoint, StartPosition: 0, Length: 3), tokens[index: 0]);
        Assert.Equal(new Token('%', TokenType.Operation, StartPosition: 3, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 4, Length: 1), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader12()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "-2.1");

        Assert.Single(collection: tokens);

        Assert.Equal(new Token(-2.1, TokenType.FloatingPoint, StartPosition: 0, Length: 4), tokens[index: 0]);
    }

    [Fact]
    public void TestTokenReader13()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "5-2");

        Assert.Equal(3, tokens.Count);
        
        Assert.Equal(new Token(5, TokenType.Integer, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token('-', TokenType.Operation, StartPosition: 1, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 2, Length: 1), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader14()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "5*-2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token('*', TokenType.Operation, StartPosition: 1, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(-2, TokenType.Integer, StartPosition: 2, Length: 2), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader15()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "5*(-2)");

        Assert.Equal(5, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token('*', TokenType.Operation, StartPosition: 1, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 2, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(-2, TokenType.Integer, StartPosition: 3, Length: 2), tokens[index: 3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, StartPosition: 5, Length: 1), tokens[index: 4]);
    }

    [Fact]
    public void TestTokenReader16()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "5*-(2+43)");

        Assert.Equal(8, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token('*', TokenType.Operation, StartPosition: 1, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token('_', TokenType.Operation, StartPosition: 2, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 3, Length: 1), tokens[index: 3]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 4, Length: 1), tokens[index: 4]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 5, Length: 1), tokens[index: 5]);
        Assert.Equal(new Token(43, TokenType.Integer, StartPosition: 6, Length: 2), tokens[index: 6]);
        Assert.Equal(new Token(')', TokenType.RightBracket, StartPosition: 8, Length: 1), tokens[index: 7]);
    }

    [Fact]
    public void TestTokenReader17()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "logn(2,5)");

        Assert.Equal(6, tokens.Count);

        Assert.Equal(new Token("logn", TokenType.Text, StartPosition: 0, Length: 4), tokens[index: 0]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, StartPosition: 4, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 5, Length: 1), tokens[index: 2]);
        Assert.Equal(new Token(',', TokenType.ArgumentSeparator, StartPosition: 6, Length: 1), tokens[index: 3]);
        Assert.Equal(new Token(5, TokenType.Integer, StartPosition: 7, Length: 1), tokens[index: 4]);
        Assert.Equal(new Token(')', TokenType.RightBracket, StartPosition: 8, Length: 1), tokens[index: 5]);
    }

    [Fact]
    public void TestTokenReader18()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "var_1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var_1", TokenType.Text, StartPosition: 0, Length: 5), tokens[index: 0]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 5, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2, TokenType.Integer, StartPosition: 6, Length: 1), tokens[index: 2]);
    }
    
    [Fact]
    public void TestTokenReader20()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "2.11E-3");

        Assert.Single(collection: tokens);

        Assert.Equal(new Token(2.11E-3, TokenType.FloatingPoint, StartPosition: 0, Length: 7), tokens[index: 0]);
    }

    [Fact]
    public void TestTokenReader21()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "var_1+2.11E-3");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var_1", TokenType.Text, StartPosition: 0, Length: 5), tokens[index: 0]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 5, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(2.11E-3, TokenType.FloatingPoint, StartPosition: 6, Length: 7), tokens[index: 2]);
    }
    
    [Fact]
    public void TestTokenReader22()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "2.11e3");

        Assert.Single(collection: tokens);

        Assert.Equal(new Token(2.11E3, TokenType.FloatingPoint, StartPosition: 0, Length: 6), tokens[index: 0]);
    }

    [Fact]
    public void TestTokenReader23()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "1 * e");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(1, TokenType.Integer, StartPosition: 0, Length: 1), tokens[index: 0]);
        Assert.Equal(new Token('*', TokenType.Operation, StartPosition: 2, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token("e", TokenType.Text, StartPosition: 4, Length: 1), tokens[index: 2]);
    }

    [Fact]
    public void TestTokenReader24()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "e");

        Assert.Single(collection: tokens);

        Assert.Equal(new Token("e", TokenType.Text, StartPosition: 0, Length: 1), tokens[index: 0]);
    }

    [Fact]
    public void TestTokenReader25()
    {
        var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
        var tokens = reader.Read(formula: "2.11e3+1.23E4");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(2.11E3, TokenType.FloatingPoint, StartPosition: 0, Length: 6), tokens[index: 0]);
        Assert.Equal(new Token('+', TokenType.Operation, StartPosition: 6, Length: 1), tokens[index: 1]);
        Assert.Equal(new Token(1.23E4, TokenType.FloatingPoint, StartPosition: 7, Length: 6), tokens[index: 2]);
    }
    
    [Fact]
    public void TestTokenReader_ThrowsIfInvalid1()
    {
        var ex = Assert.Throws<ParseException>(testCode: () =>
        {
            var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
            reader.Read(formula: "$1+$2+$3");
        });
        Assert.Equal("Invalid token \"$\" detected at position 0.", ex.Message);
    }
    
    [Fact]
    public void TestTokenReader_ThrowsIfInvalid2()
    {
        var ex = Assert.Throws<ParseException>(testCode: () =>
        {
            var reader = new TokenReader(cultureInfo: CultureInfo.InvariantCulture);
            reader.Read(formula: "2.11E-E3");
        });
        Assert.Equal("Invalid token \"E\" detected at position 6.", ex.Message);
    }
}