using System;

namespace Jace.Tokenizer;

/// <summary>
/// Represents an input token
/// </summary>
public readonly record struct Token
{
    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="tokenType">The type of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    [Obsolete("Use the other constructors instead")]
    public Token(object value, TokenType tokenType, ushort startPosition = 0, ushort length = 0)
    {
        Value = value;
        TokenType = tokenType;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>
    /// Represents an input token
    /// </summary>
    /// <param name="value">The value of the token</param>
    /// <param name="tokenType">The type of the token</param>
    /// <param name="startPosition">The start position of the token in the input function text</param>
    /// <param name="length">The length of token in the input function text</param>
    public Token(char value, TokenType tokenType, ushort startPosition = 0, ushort length = 0)
    {
        if (tokenType is not (TokenType.ArgumentSeparator or TokenType.LeftBracket or TokenType.RightBracket
                           or TokenType.Operation))
            throw new InvalidOperationException("Invalid token type for char value");
        Value = value;
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
    public Token(int value, ushort startPosition = 0, ushort length = 0)
    {
        Value = value;
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
    public Token(double value, ushort startPosition = 0, ushort length = 0)
    {
        Value = value;
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
    public Token(string value, ushort startPosition = 0, ushort length = 0)
    {
        Value = value;
        TokenType = TokenType.Text;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>The value of the token</summary>
    public object Value { get; init; }

    /// <summary>The type of the token</summary>
    public TokenType TokenType { get; init; }

    /// <summary>The start position of the token in the input function text</summary>
    public ushort StartPosition { get; init; }

    /// <summary>The length of token in the input function text</summary>
    public ushort Length { get; init; }
    
    public int IntValue => (int)Value;
    public string StringValue => (string)Value;
    public double FloatValue => (double)Value;
    public char CharValue => (char)Value;
}
