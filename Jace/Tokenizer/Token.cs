namespace Jace.Tokenizer;

/// <summary>
/// Represents an input token
/// </summary>
/// <param name="TokenType">The type of the token</param>
/// <param name="Value">The value of the token</param>
/// <param name="StartPosition">The start position of the token in the input function text</param>
/// <param name="Length">The length of token in the input function text</param>
public readonly record struct Token(object Value, TokenType TokenType, int StartPosition = 0, int Length = 0);