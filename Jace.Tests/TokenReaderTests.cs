using System.Globalization;
using Jace.Tokenizer;
using Xunit;

namespace Jace.Tests;

public sealed class TokenReaderTests
{
    [Fact]
    public void TestTokenReader1()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("42+31");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(42, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)2, tokens[0].Length);

        Assert.Equal('+', tokens[1].Value);
        Assert.Equal((uint)2, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(31, tokens[2].Value);
        Assert.Equal((uint)3, tokens[2].StartPosition);
        Assert.Equal((uint)2, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader2()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("(42+31)");

        Assert.Equal(5, tokens.Count);

        Assert.Equal('(', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal(42, tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)2, tokens[1].Length);

        Assert.Equal('+', tokens[2].Value);
        Assert.Equal((uint)3, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(31, tokens[3].Value);
        Assert.Equal((uint)4, tokens[3].StartPosition);
        Assert.Equal((uint)2, tokens[3].Length);

        Assert.Equal(')', tokens[4].Value);
        Assert.Equal((uint)6, tokens[4].StartPosition);
        Assert.Equal((uint)1, tokens[4].Length);
    }

    [Fact]
    public void TestTokenReader3()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(42+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal('(', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal(42, tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)2, tokens[1].Length);

        Assert.Equal('+', tokens[2].Value);
        Assert.Equal((uint)3, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(31.0, tokens[3].Value);
        Assert.Equal((uint)4, tokens[3].StartPosition);
        Assert.Equal((uint)4, tokens[3].Length);
    }

    [Fact]
    public void TestTokenReader4()
    {
        var reader = new TokenReader();
        var tokens = reader.Read("(42+ 8) *2");

        Assert.Equal(7, tokens.Count);

        Assert.Equal('(', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal(42, tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)2, tokens[1].Length);

        Assert.Equal('+', tokens[2].Value);
        Assert.Equal((uint)3, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(8, tokens[3].Value);
        Assert.Equal((uint)5, tokens[3].StartPosition);
        Assert.Equal((uint)1, tokens[3].Length);

        Assert.Equal(')', tokens[4].Value);
        Assert.Equal((uint)6, tokens[4].StartPosition);
        Assert.Equal((uint)1, tokens[4].Length);

        Assert.Equal('*', tokens[5].Value);
        Assert.Equal((uint)8, tokens[5].StartPosition);
        Assert.Equal((uint)1, tokens[5].Length);

        Assert.Equal(2, tokens[6].Value);
        Assert.Equal((uint)9, tokens[6].StartPosition);
        Assert.Equal((uint)1, tokens[6].Length);
    }

    [Fact]
    public void TestTokenReader5()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(42.87+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal('(', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal(42.87, tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)5, tokens[1].Length);

        Assert.Equal('+', tokens[2].Value);
        Assert.Equal((uint)6, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(31.0, tokens[3].Value);
        Assert.Equal((uint)7, tokens[3].StartPosition);
        Assert.Equal((uint)4, tokens[3].Length);
    }

    [Fact]
    public void TestTokenReader6()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("(var+31.0");

        Assert.Equal(4, tokens.Count);

        Assert.Equal('(', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal("var", tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)3, tokens[1].Length);

        Assert.Equal('+', tokens[2].Value);
        Assert.Equal((uint)4, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(31.0, tokens[3].Value);
        Assert.Equal((uint)5, tokens[3].StartPosition);
        Assert.Equal((uint)4, tokens[3].Length);
    }

    [Fact]
    public void TestTokenReader7()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);

        var tokens = reader.Read("varb");

        Assert.Single(tokens);

        Assert.Equal("varb", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)4, tokens[0].Length);
    }

    [Fact]
    public void TestTokenReader8()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("varb(");

        Assert.Equal(2, tokens.Count);

        Assert.Equal("varb", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)4, tokens[0].Length);

        Assert.Equal('(', tokens[1].Value);
        Assert.Equal((uint)4, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);
    }

    [Fact]
    public void TestTokenReader9()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("+varb(");

        Assert.Equal(3, tokens.Count);

        Assert.Equal('+', tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal("varb", tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)4, tokens[1].Length);

        Assert.Equal('(', tokens[2].Value);
        Assert.Equal((uint)5, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader10()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal("var1", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)4, tokens[0].Length);

        Assert.Equal('+', tokens[1].Value);
        Assert.Equal((uint)4, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(2, tokens[2].Value);
        Assert.Equal((uint)5, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader11()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5.1%2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(5.1, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)3, tokens[0].Length);

        Assert.Equal('%', tokens[1].Value);
        Assert.Equal((uint)3, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(2, tokens[2].Value);
        Assert.Equal((uint)4, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader12()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("-2.1");

        Assert.Single(tokens);

        Assert.Equal(-2.1, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)4, tokens[0].Length);
    }

    [Fact]
    public void TestTokenReader13()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5-2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(5, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal('-', tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(2, tokens[2].Value);
        Assert.Equal((uint)2, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader14()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*-2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(5, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal('*', tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(-2, tokens[2].Value);
        Assert.Equal((uint)2, tokens[2].StartPosition);
        Assert.Equal((uint)2, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader15()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*(-2)");

        Assert.Equal(5, tokens.Count);

        Assert.Equal(5, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal('*', tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal('(', tokens[2].Value);
        Assert.Equal((uint)2, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(-2, tokens[3].Value);
        Assert.Equal((uint)3, tokens[3].StartPosition);
        Assert.Equal((uint)2, tokens[3].Length);

        Assert.Equal(')', tokens[4].Value);
        Assert.Equal((uint)5, tokens[4].StartPosition);
        Assert.Equal((uint)1, tokens[4].Length);
    }

    [Fact]
    public void TestTokenReader16()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("5*-(2+43)");

        Assert.Equal(8, tokens.Count);

        Assert.Equal(5, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal('*', tokens[1].Value);
        Assert.Equal((uint)1, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal('_', tokens[2].Value);
        Assert.Equal((uint)2, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal('(', tokens[3].Value);
        Assert.Equal((uint)3, tokens[3].StartPosition);
        Assert.Equal((uint)1, tokens[3].Length);

        Assert.Equal(2, tokens[4].Value);
        Assert.Equal((uint)4, tokens[4].StartPosition);
        Assert.Equal((uint)1, tokens[4].Length);

        Assert.Equal('+', tokens[5].Value);
        Assert.Equal((uint)5, tokens[5].StartPosition);
        Assert.Equal((uint)1, tokens[5].Length);

        Assert.Equal(43, tokens[6].Value);
        Assert.Equal((uint)6, tokens[6].StartPosition);
        Assert.Equal((uint)2, tokens[6].Length);

        Assert.Equal(')', tokens[7].Value);
        Assert.Equal((uint)8, tokens[7].StartPosition);
        Assert.Equal((uint)1, tokens[7].Length);
    }

    [Fact]
    public void TestTokenReader17()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("logn(2,5)");

        Assert.Equal(6, tokens.Count);

        Assert.Equal("logn", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)4, tokens[0].Length);

        Assert.Equal('(', tokens[1].Value);
        Assert.Equal((uint)4, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);
        Assert.Equal(TokenType.LeftBracket, tokens[1].TokenType);

        Assert.Equal(2, tokens[2].Value);
        Assert.Equal((uint)5, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);

        Assert.Equal(',', tokens[3].Value);
        Assert.Equal((uint)6, tokens[3].StartPosition);
        Assert.Equal((uint)1, tokens[3].Length);
        Assert.Equal(TokenType.ArgumentSeparator, tokens[3].TokenType);

        Assert.Equal(5, tokens[4].Value);
        Assert.Equal((uint)7, tokens[4].StartPosition);
        Assert.Equal((uint)1, tokens[4].Length);

        Assert.Equal(')', tokens[5].Value);
        Assert.Equal((uint)8, tokens[5].StartPosition);
        Assert.Equal((uint)1, tokens[5].Length);
        Assert.Equal(TokenType.RightBracket, tokens[5].TokenType);
    }

    [Fact]
    public void TestTokenReader18()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var_1+2");

        Assert.Equal(3, tokens.Count);

        Assert.Equal("var_1", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)5, tokens[0].Length);

        Assert.Equal('+', tokens[1].Value);
        Assert.Equal((uint)5, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(2, tokens[2].Value);
        Assert.Equal((uint)6, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader19()
    {
        Assert.Throws<ParseException>(() =>
        {
            var reader = new TokenReader(CultureInfo.InvariantCulture);
            reader.Read("$1+$2+$3");
        });
    }

    [Fact]
    public void TestTokenReader20()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11E-3");

        Assert.Single(tokens);

        Assert.Equal(2.11E-3, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)7, tokens[0].Length);
    }

    [Fact]
    public void TestTokenReader21()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("var_1+2.11E-3");

        Assert.Equal(3, tokens.Count);

        Assert.Equal("var_1", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)5, tokens[0].Length);

        Assert.Equal('+', tokens[1].Value);
        Assert.Equal((uint)5, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal(2.11E-3, tokens[2].Value);
        Assert.Equal((uint)6, tokens[2].StartPosition);
        Assert.Equal((uint)7, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader22()
    {
        Assert.Throws<ParseException>(() =>
        {
            var reader = new TokenReader(CultureInfo.InvariantCulture);
            reader.Read("2.11E-E3");
        });
    }

    [Fact]
    public void TestTokenReader23()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11e3");

        Assert.Single(tokens);

        Assert.Equal(2.11E3, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)6, tokens[0].Length);
    }

    [Fact]
    public void TestTokenReader24()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("1 * e");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(1, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);

        Assert.Equal('*', tokens[1].Value);
        Assert.Equal((uint)2, tokens[1].StartPosition);
        Assert.Equal((uint)1, tokens[1].Length);

        Assert.Equal("e", tokens[2].Value);
        Assert.Equal((uint)4, tokens[2].StartPosition);
        Assert.Equal((uint)1, tokens[2].Length);
    }

    [Fact]
    public void TestTokenReader25()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("e");

        Assert.Single(tokens);

        Assert.Equal("e", tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)1, tokens[0].Length);
    }

    [Fact]
    public void TestTokenReader26()
    {
        var reader = new TokenReader(CultureInfo.InvariantCulture);
        var tokens = reader.Read("2.11e3+1.23E4");

        Assert.Equal(3, tokens.Count);

        Assert.Equal(2.11E3, tokens[0].Value);
        Assert.Equal((uint)0, tokens[0].StartPosition);
        Assert.Equal((uint)6, tokens[0].Length);

        Assert.Equal('+', tokens[1].Value);

        Assert.Equal(1.23E4, tokens[2].Value);
        Assert.Equal((uint)7, tokens[2].StartPosition);
        Assert.Equal((uint)6, tokens[2].Length);
    }
}