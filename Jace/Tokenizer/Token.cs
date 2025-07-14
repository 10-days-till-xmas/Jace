namespace Jace.Tokenizer;

/// <summary>
/// Represents an input token
/// </summary>
/// <param name="Value">The value of the token.</param>
/// <param name="TokenType">The type of the token.</param>
/// <param name="StartPosition">The start position of the token in the input function text.</param>
/// <param name="Length">The length of token in the input function text. Default value is <c>1</c>, assuming a char</param>
public record struct Token(
    object Value,
    TokenType TokenType,
    int StartPosition,
    int Length = 1);

// TODO: Check if Length and StartPosition can be removed

// StartPosition is only used for error messages, and Length is not used at all. 