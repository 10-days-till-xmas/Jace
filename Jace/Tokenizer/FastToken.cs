using System;
using System.Runtime.InteropServices;

namespace Jace.Tokenizer;

[StructLayout(LayoutKind.Explicit)]
public readonly record struct FastToken
{
    /// <summary>The start position of the token in the input function text</summary>
    [field: FieldOffset(0)] // bytes 0,1
    public ushort StartPosition { get; init; }
    /// <summary>The length of token in the input function text</summary>
    [field: FieldOffset(2)] // bytes 2,3
    public ushort Length { get; init; }
    /// <summary>The type of the token</summary>
    [field: FieldOffset(4)] // bytes 4
    public TokenType TokenType { get; }
    [FieldOffset(5)] // bytes 5,6,7,8,9,10,11,12
    private readonly double _floatValue_unsafe;
    [FieldOffset(5)] // bytes 5,6,7,8
    private readonly int _intValue_unsafe;
    [FieldOffset(5)] // bytes 5,6
    private readonly char _charValue_unsafe;
    [FieldOffset(16)] // bytes 12,13,14,15 (reference)
    private readonly string _stringValue_unsafe = null!;

    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="tokenType">The type of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    public FastToken(char value, TokenType tokenType, ushort startPosition = 0, ushort length = 0)
    {
        if (tokenType is not (TokenType.ArgumentSeparator or TokenType.LeftBracket or TokenType.RightBracket
             or TokenType.Operation))
            throw new InvalidOperationException("Invalid token type for char value");
        _charValue_unsafe = value;
        TokenType = tokenType;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    public FastToken(int value, ushort startPosition = 0, ushort length = 0)
    {
        _intValue_unsafe = value;
        TokenType = TokenType.Integer;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    public FastToken(double value, ushort startPosition = 0, ushort length = 0)
    {
        _floatValue_unsafe = value;
        TokenType = TokenType.FloatingPoint;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    public FastToken(string value, ushort startPosition = 0, ushort length = 0)
    {
        _stringValue_unsafe = value;
        TokenType = TokenType.Text;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>The value of the token</summary>
    public object Value
    {
        get
        {
            return TokenType switch
            {
                TokenType.Integer       => _intValue_unsafe,
                TokenType.FloatingPoint => _floatValue_unsafe,
                TokenType.Operation or TokenType.LeftBracket or TokenType.RightBracket
                 or TokenType.ArgumentSeparator => _charValue_unsafe,
                TokenType.Text  => _stringValue_unsafe,
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
}
