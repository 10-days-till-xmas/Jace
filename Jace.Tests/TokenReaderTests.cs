using System.Globalization;
using Jace.Tokenizer;
using Xunit;

namespace Jace.Tests;

public sealed class TokenReaderTests
{
    [Fact]
    public void TestTokenReader01()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("42+31");

        Assert.Equal(3, tokens.Count);
        
        Assert.Equal(new Token(42, TokenType.Integer, 0, 2), tokens[0]);
        Assert.Equal(new Token('+', TokenType.Operation, 2, 1), tokens[1]);
        Assert.Equal(new Token(31, TokenType.Integer, 3, 2), tokens[2]);
    }

    [Fact]
    public void TestTokenReader02()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("(42+31)");

        Assert.Equal(5, tokens.Count);
        
        Assert.Equal(new Token('(', TokenType.LeftBracket, 0, 1), tokens[0]);
        Assert.Equal(new Token(42, TokenType.Integer, 1, 2), tokens[1]);
        Assert.Equal(new Token('+', TokenType.Operation, 3, 1), tokens[2]);
        Assert.Equal(new Token(31, TokenType.Integer, 4, 2), tokens[3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, 6, 1), tokens[4]);
    }

    [Fact]
    public void TestTokenReader03()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(42+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, 0, 1), tokens[0]);
        Assert.Equal(new Token(42, TokenType.Integer, 1, 2), tokens[1]);
        Assert.Equal(new Token('+', TokenType.Operation, 3, 1), tokens[2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, 4, 4), tokens[3]);
    }

    [Fact]
    public void TestTokenReader04()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("(42+ 8) *2");

        Assert.Equal(7, tokens.Count);
        
        Assert.Equal(new Token('(', TokenType.LeftBracket, 0, 1), tokens[0]);
        Assert.Equal(new Token(42, TokenType.Integer, 1, 2), tokens[1]);
        Assert.Equal(new Token('+', TokenType.Operation, 3, 1), tokens[2]);
        Assert.Equal(new Token(8, TokenType.Integer, 5, 1), tokens[3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, 6, 1), tokens[4]);
        Assert.Equal(new Token('*', TokenType.Operation, 8, 1), tokens[5]);
        Assert.Equal(new Token(2, TokenType.Integer, 9, 1), tokens[6]);
    }

    [Fact]
    public void TestTokenReader05()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(42.87+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, 0, 1), tokens[0]);
        Assert.Equal(new Token(42.87, TokenType.FloatingPoint, 1, 5), tokens[1]);
        Assert.Equal(new Token('+', TokenType.Operation, 6, 1), tokens[2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, 7, 4), tokens[3]);
    }

    [Fact]
    public void TestTokenReader06()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(var+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal(new Token('(', TokenType.LeftBracket, 0, 1), tokens[0]);
        Assert.Equal(new Token("var", TokenType.Text, 1, 3), tokens[1]);
        Assert.Equal(new Token('+', TokenType.Operation, 4, 1), tokens[2]);
        Assert.Equal(new Token(31.0, TokenType.FloatingPoint, 5, 4), tokens[3]);
    }

    [Fact]
    public void TestTokenReader07()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);

        var tokens = reader.Read("varb");

        Assert.Single(tokens);

        Assert.Equal(new Token("varb", TokenType.Text, 0, 4), tokens[0]);
    }

    [Fact]
    public void TestTokenReader08()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("varb(");

        Assert.Equal(2, tokens.Count);

        Assert.Equal(new Token("varb", TokenType.Text, 0, 4), tokens[0]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, 4, 1), tokens[1]);
    }

    [Fact]
    public void TestTokenReader09()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("+varb(");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token('+', TokenType.Operation, 0, 1), tokens[0]);
        Assert.Equal(new Token("varb", TokenType.Text, 1, 4), tokens[1]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, 5, 1), tokens[2]);
    }

    [Fact]
    public void TestTokenReader10()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var1", TokenType.Text, 0, 4), tokens[0]);
        Assert.Equal(new Token('+', TokenType.Operation, 4, 1), tokens[1]);
        Assert.Equal(new Token(2, TokenType.Integer, 5, 1), tokens[2]);
    }

    [Fact]
    public void TestTokenReader11()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5.1%2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(5.1, TokenType.FloatingPoint, 0, 3), tokens[0]);
        Assert.Equal(new Token('%', TokenType.Operation, 3, 1), tokens[1]);
        Assert.Equal(new Token(2, TokenType.Integer, 4, 1), tokens[2]);
    }

    [Fact]
    public void TestTokenReader12()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("-2.1");

        Assert.Single(tokens);

        Assert.Equal(new Token(-2.1, TokenType.FloatingPoint, 0, 4), tokens[0]);
    }

    [Fact]
    public void TestTokenReader13()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5-2");

        Assert.Equal(3, tokens.Count);
        
        Assert.Equal(new Token(5, TokenType.Integer, 0, 1), tokens[0]);
        Assert.Equal(new Token('-', TokenType.Operation, 1, 1), tokens[1]);
        Assert.Equal(new Token(2, TokenType.Integer, 2, 1), tokens[2]);
    }

    [Fact]
    public void TestTokenReader14()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*-2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, 0, 1), tokens[0]);
        Assert.Equal(new Token('*', TokenType.Operation, 1, 1), tokens[1]);
        Assert.Equal(new Token(-2, TokenType.Integer, 2, 2), tokens[2]);
    }

    [Fact]
    public void TestTokenReader15()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*(-2)");

        Assert.Equal(5, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, 0, 1), tokens[0]);
        Assert.Equal(new Token('*', TokenType.Operation, 1, 1), tokens[1]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, 2, 1), tokens[2]);
        Assert.Equal(new Token(-2, TokenType.Integer, 3, 2), tokens[3]);
        Assert.Equal(new Token(')', TokenType.RightBracket, 5, 1), tokens[4]);
    }

    [Fact]
    public void TestTokenReader16()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*-(2+43)");

        Assert.Equal(8, tokens.Count);

        Assert.Equal(new Token(5, TokenType.Integer, 0, 1), tokens[0]);
        Assert.Equal(new Token('*', TokenType.Operation, 1, 1), tokens[1]);
        Assert.Equal(new Token('_', TokenType.Operation, 2, 1), tokens[2]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, 3, 1), tokens[3]);
        Assert.Equal(new Token(2, TokenType.Integer, 4, 1), tokens[4]);
        Assert.Equal(new Token('+', TokenType.Operation, 5, 1), tokens[5]);
        Assert.Equal(new Token(43, TokenType.Integer, 6, 2), tokens[6]);
        Assert.Equal(new Token(')', TokenType.RightBracket, 8, 1), tokens[7]);
    }

    [Fact]
    public void TestTokenReader17()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("logn(2,5)");

        Assert.Equal(6, tokens.Count);

        Assert.Equal(new Token("logn", TokenType.Text, 0, 4), tokens[0]);
        Assert.Equal(new Token('(', TokenType.LeftBracket, 4, 1), tokens[1]);
        Assert.Equal(new Token(2, TokenType.Integer, 5, 1), tokens[2]);
        Assert.Equal(new Token(',', TokenType.ArgumentSeparator, 6, 1), tokens[3]);
        Assert.Equal(new Token(5, TokenType.Integer, 7, 1), tokens[4]);
        Assert.Equal(new Token(')', TokenType.RightBracket, 8, 1), tokens[5]);
    }

    [Fact]
    public void TestTokenReader18()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var_1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var_1", TokenType.Text, 0, 5), tokens[0]);
        Assert.Equal(new Token('+', TokenType.Operation, 5, 1), tokens[1]);
        Assert.Equal(new Token(2, TokenType.Integer, 6, 1), tokens[2]);
    }
    
    [Fact]
    public void TestTokenReader20()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11E-3");

        Assert.Single(tokens);

        Assert.Equal(new Token(2.11E-3, TokenType.FloatingPoint, 0, 7), tokens[0]);
    }

    [Fact]
    public void TestTokenReader21()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var_1+2.11E-3");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token("var_1", TokenType.Text, 0, 5), tokens[0]);
        Assert.Equal(new Token('+', TokenType.Operation, 5, 1), tokens[1]);
        Assert.Equal(new Token(2.11E-3, TokenType.FloatingPoint, 6, 7), tokens[2]);
    }
    
    [Fact]
    public void TestTokenReader22()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11e3");

        Assert.Single(tokens);

        Assert.Equal(new Token(2.11E3, TokenType.FloatingPoint, 0, 6), tokens[0]);
    }

    [Fact]
    public void TestTokenReader23()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("1 * e");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(1, TokenType.Integer, 0, 1), tokens[0]);
        Assert.Equal(new Token('*', TokenType.Operation, 2, 1), tokens[1]);
        Assert.Equal(new Token("e", TokenType.Text, 4, 1), tokens[2]);
    }

    [Fact]
    public void TestTokenReader24()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("e");

        Assert.Single(tokens);

        Assert.Equal(new Token("e", TokenType.Text, 0, 1), tokens[0]);
    }

    [Fact]
    public void TestTokenReader25()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11e3+1.23E4");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(new Token(2.11E3, TokenType.FloatingPoint, 0, 6), tokens[0]);
        Assert.Equal(new Token('+', TokenType.Operation, 6, 1), tokens[1]);
        Assert.Equal(new Token(1.23E4, TokenType.FloatingPoint, 7, 6), tokens[2]);
    }
    
    [Fact]
    public void TestTokenReader_ThrowsIfInvalid1()
    {
        var ex = Assert.Throws<ParseException>(() =>
        {
            var reader = new TokenReader(CultureInfo.InvariantCulture);
            reader.Read("$1+$2+$3");
        });
        Assert.Equal("Invalid token \"$\" detected at position 0.", ex.Message);
    }
    
    [Fact]
    public void TestTokenReader_ThrowsIfInvalid2()
    {
        var ex = Assert.Throws<ParseException>(() =>
        {
            var reader = new TokenReader(CultureInfo.InvariantCulture);
            reader.Read("2.11E-E3");
        });
        Assert.Equal("Invalid token \"E\" detected at position 6.", ex.Message);
    }
}