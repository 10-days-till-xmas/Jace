namespace Jace.Tokenizer;

/// <summary>
/// Represents an input token
/// </summary>
/// <param name="Value">The value of the token.</param>
/// <param name="TokenType">The type of the token.</param>
/// <param name="StartPosition">The start position of the token in the input function text.</param>
/// <param name="Length">The length of token in the input function text.</param>
public record struct Token(
    object Value,
    TokenType TokenType,
    int StartPosition,
    int Length);
// TODO: Check if Length can be removed