namespace Yace.Tokenizer;

public enum TokenType : byte
{
    Integer,
    FloatingPoint,
    Text,
    Operation,
    LeftBracket,
    RightBracket,
    ArgumentSeparator
}