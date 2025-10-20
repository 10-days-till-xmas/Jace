using System;
using System.Runtime.InteropServices;

namespace Jace.Tokenizer;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct FastToken
{
    public FastToken(double floatValue, int startPosition = 0, int length = 0)
    {
        _floatValue_unsafe = floatValue;
        TokenType = TokenType.FloatingPoint;
        StartPosition = startPosition;
        Length = length;
    }
    public FastToken(int intValue, int startPosition = 0, int length = 0)
    {
        _intValue_unsafe = intValue;
        TokenType = TokenType.Integer;
        StartPosition = startPosition;
        Length = length;
    }
    public FastToken(string stringValue, TokenType tokenType, int startPosition = 0, int length = 0)
    {
        _stringValue_unsafe = stringValue;
        TokenType = tokenType;
        StartPosition = startPosition;
        Length = length;
    }

    [FieldOffset(0)]
    public readonly int StartPosition;
    [FieldOffset(4)]
    public readonly int Length;
    [FieldOffset(8)]
    public readonly TokenType TokenType;
    [FieldOffset(12)]
    private readonly double _floatValue_unsafe;
    [FieldOffset(12)]
    private readonly int _intValue_unsafe;
    [FieldOffset(12)]
    private readonly char _charValue_unsafe;
    [FieldOffset(12)]
    private readonly string _stringValue_unsafe = null!;

    public object Value
    {
        get
        {
            return TokenType switch
            {
                TokenType.Integer       => _intValue_unsafe,
                TokenType.FloatingPoint => _floatValue_unsafe,
                TokenType.Operation => _charValue_unsafe,
                TokenType.Text or TokenType.LeftBracket or TokenType.RightBracket
                 or TokenType.ArgumentSeparator => _stringValue_unsafe,
                _ => throw new InvalidOperationException("Token does not have a value")
            };
        }
    }

    public double FloatValue => TokenType == TokenType.FloatingPoint
                                    ? _floatValue_unsafe
                                    : throw new InvalidOperationException("Token isn't a float");
    public int IntValue => TokenType == TokenType.Integer
                               ? _intValue_unsafe
                               : throw new InvalidOperationException("Token isn't an integer");
    public string StringValue => TokenType is TokenType.Text
                                     ? _stringValue_unsafe
                                     : throw new InvalidOperationException("Token isn't a string");
    public char CharValue => TokenType is TokenType.Operation or TokenType.LeftBracket or TokenType.RightBracket or TokenType.ArgumentSeparator
                                 ? _charValue_unsafe
                                 : throw new InvalidOperationException("Token isn't a char");

    public static implicit operator Token(FastToken fastToken) => new(fastToken.Value, fastToken.TokenType, fastToken.StartPosition, fastToken.Length);

    public static implicit operator FastToken(Token token)
    {
        return token.TokenType switch
        {
            TokenType.Integer       => new FastToken((int)token.Value, token.StartPosition, token.Length),
            TokenType.FloatingPoint => new FastToken((double)token.Value, token.StartPosition, token.Length),
            TokenType.Text or TokenType.Operation or TokenType.LeftBracket or TokenType.RightBracket
             or TokenType.ArgumentSeparator => new FastToken((string)token.Value, token.TokenType, token.StartPosition, token.Length),
            _ => throw new InvalidOperationException("Invalid token type for conversion to FastToken")
        };
    }
}