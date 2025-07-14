using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Jace.Util;

namespace Jace.Tokenizer;
#if NET5_0_OR_GREATER
public partial class TokenReader
{
    private List<Token> ReadAsSpan(string formula)
    {
        if (string.IsNullOrEmpty(formula))
            throw new ArgumentNullException(nameof(formula));

        var tokens = new List<Token>();
        var characters = formula.AsSpan();
        
        var isFormulaSubPart = true;
        var isScientific = false;
        
        var i = 0;
        while (characters.TryDequeue(ref i, out var currentChar))
        {
            i--;
            if (currentChar == ' ')
            {
                // Skip whitespace
            }
            else if (IsPartOfNumeric(currentChar, true, isFormulaSubPart))
            {
                var numericToken = ParseNumeric(characters, ref i, ref isFormulaSubPart, ref isScientific);
                tokens.Add(numericToken);
            }
            else if (IsPartOfVariable(currentChar, true))
            {
                var variableToken = ParseVariableName(characters, ref i, out isFormulaSubPart);
                tokens.Add(variableToken);
            } 
            else if (currentChar == argumentSeparator)
            {
                var argumentSeparatorToken =
                    new Token(currentChar, TokenType.ArgumentSeparator, StartPosition: i, Length: 1);
                tokens.Add(argumentSeparatorToken);
                isFormulaSubPart = false;
            }
            else
            {
                var operationToken = AppendOperationToken(characters, ref i, tokens, ref isFormulaSubPart);
                tokens.Add(operationToken);
            }

            i++;
        }

        return tokens;
    }
    
    private Token ParseNumeric(ReadOnlySpan<char> characters, ref int i, ref bool isFormulaSubPart, ref bool isScientific)
    {
        var buffer = new StringBuilder();
        var startPosition = i;
        buffer.Append(characters.Dequeue(ref i)); 
            
        var _isFormulaSubPart = isFormulaSubPart;

        while (characters.DequeueIf(ref i, out var currentChar,
                c => IsPartOfNumeric(c, false, _isFormulaSubPart)))
        {
            if (isScientific && IsScientificNotation(currentChar))
                throw new ParseException($"Invalid token \"{currentChar}\" detected at position {i-1}.");
            if (IsScientificNotation(currentChar))
            {
                isScientific = true;
                buffer.Append(currentChar); // Append 'e' or 'E'
                if (characters.DequeueIf(ref i, out var minus, c => c == '-')) 
                    buffer.Append(minus);
            }
            else 
            {
                buffer.Append(currentChar); // add digit or decimal separator
            }
        }
        
        var numericToken = buffer.ToString();
        var length = i - startPosition;
        i--;
        // Verify if we don't have an int
        if (int.TryParse(numericToken, out var intValue))
        {
            isFormulaSubPart = false;
            return new Token(intValue, TokenType.Integer, startPosition, length);
        }
        if (double.TryParse(numericToken, 
                NumberStyles.Float | NumberStyles.AllowThousands, cultureInfo, 
                out var doubleValue))
        {
            isScientific = false;
            isFormulaSubPart = false;
            return new Token(doubleValue, TokenType.FloatingPoint, startPosition, length);
        }
        if (numericToken == "-")
        {
            // Verify if we have a unary minus, we use the token '_' for a unary minus in the AST builder
            return new Token('_', TokenType.Operation, startPosition, Length: 1);
        }

        throw new ParseException($"Invalid numeric token \"{buffer}\" detected at position {startPosition}.");
    }
    
    private static Token ParseVariableName(ReadOnlySpan<char> characters, ref int i, out bool isFormulaSubPart)
    {
        var startPosition = i;
        var sb = new StringBuilder();
        // buffer.Append(characters.Dequeue(ref i));
        
        while (characters.DequeueIf(ref i, out var currentChar, 
                   c=>IsPartOfVariable(c, false))) 
            sb.Append(currentChar);
        i--; // decrement i to point to the last character of the variable name
             // (else it would go unread at the end of each loop)
        var varName = sb.ToString();
        isFormulaSubPart = false;
        var token = new Token(varName, TokenType.Text, startPosition, Length: 1 + i - startPosition);
        
        return token;
    }
    
    private static Token AppendOperationToken(ReadOnlySpan<char> characters, ref int i, List<Token> tokens, ref bool isFormulaSubPart)
    {
        var doSkip = false;
        Token token;
        switch (characters[i])
        { 
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
                token = IsUnaryMinus(characters[i], tokens)
                        ? new Token('_', TokenType.Operation, StartPosition: i, Length: 1)
                        : new Token(characters[i], TokenType.Operation, StartPosition: i, Length: 1);
                isFormulaSubPart = true;
                break;
            case '(':
                token = new Token(characters[i], TokenType.LeftBracket, StartPosition: i, Length: 1);
                isFormulaSubPart = true;
                break;
            case ')':
                token = new Token(characters[i], TokenType.RightBracket, StartPosition: i, Length: 1);
                isFormulaSubPart = false;
                break;
            case '<':
                if (i + 1 < characters.Length && characters[i + 1] == '=')
                {
                    token = new Token('≤', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                }
                else
                    token = new Token('<', TokenType.Operation, StartPosition: i, Length: 1);
                isFormulaSubPart = false;
                break;
            case '>':
                if (i + 1 < characters.Length && characters[i + 1] == '=')
                {
                    token = new Token('≥', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                }
                else
                    token = new Token('>', TokenType.Operation, StartPosition: i, Length: 1);
                isFormulaSubPart = false;
                break;
            case '!':
                if (i + 1 < characters.Length && characters[i + 1] == '=')
                {
                    token = new Token('≠', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                    isFormulaSubPart = false;
                }
                else
                    throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                break;
            case '&':
                if (i + 1 < characters.Length && characters[i + 1] == '&')
                {
                    token = new Token('&', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                    isFormulaSubPart = false;
                }
                else
                    throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                break;
            case '|':
                if (i + 1 < characters.Length && characters[i + 1] == '|')
                {
                    token = new Token('|', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                    isFormulaSubPart = false;
                }
                else
                    throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                break;
            case '=':
                if (i + 1 < characters.Length && characters[i + 1] == '=')
                {
                    token = new Token('=', TokenType.Operation, StartPosition: i, Length: 2);
                    doSkip = true;
                    isFormulaSubPart = false;
                }
                else
                    throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
                break;
            default:
                throw new ParseException($"Invalid token \"{characters[i]}\" detected at position {i}.");
        }

        if (doSkip)
        {
            i++; // index is now on the last operation token character (e.g., on the '=' in '!=')
        }

        return token;
    }
    
}
#endif