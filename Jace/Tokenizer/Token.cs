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
    public Token(object value, TokenType tokenType, uint startPosition = 0, uint length = 0)
    {
        Value = value;
        TokenType = tokenType;
        StartPosition = startPosition;
        Length = length;
    }

    /// <summary>The value of the token</summary>
    public object Value { get; init; }

    /// <summary>The type of the token</summary>
    public TokenType TokenType { get; init; }

    /// <summary>The start position of the token in the input function text</summary>
    public uint StartPosition { get; init; }

    /// <summary>The length of token in the input function text</summary>
    public uint Length { get; init; }
}