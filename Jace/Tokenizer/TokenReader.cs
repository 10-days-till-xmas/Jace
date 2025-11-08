using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jace.Tokenizer;

/// <summary>
/// A token reader that converts the input string in a list of tokens.
/// </summary>
public sealed class TokenReader(CultureInfo cultureInfo)
{
    private readonly CultureInfo cultureInfo = cultureInfo;
    private readonly char decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
    private readonly char argumentSeparator = cultureInfo.TextInfo.ListSeparator[0];

    public TokenReader()
        : this(CultureInfo.CurrentCulture)
    {
    }

    /// <summary>
    /// Read in the provided formula and convert it into a list of tokens that can be processed by the
    /// Abstract Syntax Tree Builder.
    /// </summary>
    /// <param name="formula">The formula that must be converted into a list of tokens.</param>
    /// <returns>The list of tokens for the provided formula.</returns>
    public List<Token> Read(string formula)
    {
        if (string.IsNullOrEmpty(formula))
            throw new ArgumentNullException(nameof(formula));

        var tokens = new List<Token>();

        var characters = formula.ToCharArray();

        var isFormulaSubPart = true;
        var isScientific = false;

        for (ushort i = 0; i < characters.Length; i++)
        {
            if (IsPartOfNumeric(characters[i], true, isFormulaSubPart))
            {
                var buffer = new StringBuilder();
                buffer.Append(characters[i]);
                //string buffer = "" + characters[i];
                var startPosition = i;

                i++;
                for (; i < characters.Length && IsPartOfNumeric(characters[i], false, isFormulaSubPart); i++)
                {
                    if (isScientific && IsScientificNotation(characters[i]))
                        throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                    if (IsScientificNotation(characters[i]))
                    {
                        isScientific = IsScientificNotation(characters[i]);

                        if (characters[i + 1] is '-' or '+')
                            buffer.Append(characters[i++]);
                    }

                    buffer.Append(characters[i]);
                }

                // Verify if we don't have an int
                if (int.TryParse(buffer.ToString(), out var intValue))
                {
                    tokens.Add(new Token(value: intValue,
                                         startPosition, (ushort)(i - startPosition)));
                    isFormulaSubPart = false;
                }
                else if (double.TryParse(buffer.ToString(), NumberStyles.Float | NumberStyles.AllowThousands,
                                         cultureInfo, out var doubleValue))
                {
                    tokens.Add(new Token(value: doubleValue, startPosition,
                                         (ushort)(i - startPosition)));
                    isScientific = false;
                    isFormulaSubPart = false;
                }
                else if (buffer.ToString() == "-")
                {
                    // Verify if we have a unary minus, we use the token '_' for a unary minus in the AST builder
                    tokens.Add(new Token(value: '_', tokenType: TokenType.Operation,
                                         startPosition: startPosition, length: 1));
                }
                // Else we skip

                if (i == characters.Length) // Last character read
                    continue;
            }

            if (IsPartOfVariable(characters[i], true))
            {
                var buffer = "" + characters[i];
                var startPosition = i;

                while (++i < characters.Length && IsPartOfVariable(characters[i], false))
                {
                    buffer += characters[i];
                }

                tokens.Add(new Token(value: buffer,
                                     startPosition, (ushort)(i - startPosition)));
                isFormulaSubPart = false;

                if (i == characters.Length)
                {
                    // Last character read
                    continue;
                }
            }
            if (characters[i] == argumentSeparator)
            {
                tokens.Add(new Token(value: characters[i], tokenType: TokenType.ArgumentSeparator,
                                     startPosition: i, length: 1));
                isFormulaSubPart = false;
            }
            else
                switch (characters[i])
                {
                    case ' ':
                        continue;
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '^':
                    case '%':
                    case '≤':
                    case '≥':
                    case '≠':
                        // We use the token '_' for a unary minus in the AST builder
                        tokens.Add(new Token(value: IsUnaryMinus(characters[i], tokens) ? '_' : characters[i],
                                             tokenType: TokenType.Operation, startPosition: i, length: 1));
                        isFormulaSubPart = true;
                        break;
                    case '(':
                        tokens.Add(new Token(value: characters[i], tokenType: TokenType.LeftBracket,
                                             startPosition: i, length: 1));
                        isFormulaSubPart = true;
                        break;
                    case ')':
                        tokens.Add(new Token(value: characters[i], tokenType: TokenType.RightBracket,
                                             startPosition: i, length: 1));
                        isFormulaSubPart = false;
                        break;
                    case '<':
                        if (i + 1 < characters.Length && characters[i + 1] == '=')
                            tokens.Add(new Token(value: '≤', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                        else
                            tokens.Add(new Token(value: '<', tokenType: TokenType.Operation,
                                                 startPosition: i, length: 1));
                        isFormulaSubPart = false;
                        break;
                    case '>':
                        if (i + 1 < characters.Length && characters[i + 1] == '=')
                            tokens.Add(new Token(value: '≥', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                        else
                            tokens.Add(new Token(value: '>', tokenType: TokenType.Operation,
                                                 startPosition: i, length: 1));
                        isFormulaSubPart = false;
                        break;
                    case '!':
                        if (i + 1 < characters.Length && characters[i + 1] == '=')
                        {
                            tokens.Add(new Token(value: '≠', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                            isFormulaSubPart = false;
                        }
                        else
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                        break;
                    case '&':
                        if (i + 1 < characters.Length && characters[i + 1] == '&')
                        {
                            tokens.Add(new Token(value: '&', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                            isFormulaSubPart = false;
                        }
                        else
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                        break;
                    case '|':
                        if (i + 1 < characters.Length && characters[i + 1] == '|')
                        {
                            tokens.Add(new Token(value: '|', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                            isFormulaSubPart = false;
                        }
                        else
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                        break;
                    case '=':
                        if (i + 1 < characters.Length && characters[i + 1] == '=')
                        {
                            tokens.Add(new Token(value: '=', tokenType: TokenType.Operation,
                                                 startPosition: i++, length: 2));
                            isFormulaSubPart = false;
                        }
                        else
                            throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");

                        break;
                    default:
                        throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                }
        }

        return tokens;
    }

    private bool IsPartOfNumeric(char character, bool isFirstCharacter, bool isFormulaSubPart)
    {
        return character == decimalSeparator
            || character is >= '0' and <= '9'
            || (isFormulaSubPart && isFirstCharacter && character == '-')
            || (!isFirstCharacter && character is 'e' or 'E');
    }

    private static bool IsPartOfVariable(char character, bool isFirstCharacter)
    {
        return character is >= 'a' and <= 'z'
            || character is >= 'A' and <= 'Z'
            || (!isFirstCharacter && character is >= '0' and <= '9' or '_');
    }

    private static bool IsUnaryMinus(char currentToken, List<Token> tokens)
    {
        if (currentToken != '-') return false;
        var previousToken = tokens[tokens.Count - 1];

        return previousToken.TokenType is not (TokenType.FloatingPoint or TokenType.Integer or TokenType.Text or TokenType.RightBracket);
    }

    private static bool IsScientificNotation(char currentToken)
    {
        return currentToken is 'e' or 'E';
    }
}
