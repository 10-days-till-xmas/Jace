using System;
using System.Collections.Generic;
using System.Linq;
using Jace.Execution;
using Jace.Operations;
using Jace.Tokenizer;
using Jace.Util;

namespace Jace;

public sealed class AstBuilder
{
    private readonly IFunctionRegistry functionRegistry;
    private readonly IConstantRegistry localConstantRegistry;
    private readonly bool caseSensitive;
    private readonly Dictionary<char, int> operationPrecedence = new();
    private readonly Stack<Operation> resultStack = new();
    private readonly Stack<Token> operatorStack = new();
    private readonly Stack<int> parameterCount = new();

    public AstBuilder(IFunctionRegistry functionRegistry, bool caseSensitive, IConstantRegistry? compiledConstants = null)
    {
        this.functionRegistry = functionRegistry ?? throw new ArgumentNullException(nameof(functionRegistry));
        localConstantRegistry = compiledConstants ?? new ConstantRegistry(caseSensitive);
        this.caseSensitive = caseSensitive;

        operationPrecedence.Add('(', 0);
        operationPrecedence.Add('&', 1);
        operationPrecedence.Add('|', 1);
        operationPrecedence.Add('<', 2);
        operationPrecedence.Add('>', 2);
        operationPrecedence.Add('≤', 2);
        operationPrecedence.Add('≥', 2);
        operationPrecedence.Add('≠', 2);
        operationPrecedence.Add('=', 2);
        operationPrecedence.Add('+', 3);
        operationPrecedence.Add('-', 3);
        operationPrecedence.Add('*', 4);
        operationPrecedence.Add('/', 4);
        operationPrecedence.Add('%', 4);
        operationPrecedence.Add('_', 6);
        operationPrecedence.Add('^', 5);
    }

    public Operation Build(IList<Token> tokens)
    {
        resultStack.Clear();
        operatorStack.Clear();

        parameterCount.Clear();

        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case TokenType.Integer:
                    resultStack.Push(new IntegerConstant((int)token.Value));
                    break;
                case TokenType.FloatingPoint:
                    resultStack.Push(new FloatingPointConstant((double)token.Value));
                    break;
                case TokenType.Text:
                    if (functionRegistry.ContainsFunctionName((string)token.Value))
                    {
                        operatorStack.Push(token);
                        parameterCount.Push(1);
                    }
                    else
                    {
                        var tokenValue = (string)token.Value;
                        if (localConstantRegistry.TryGetConstantInfo(tokenValue, out var info))
                            resultStack.Push(new FloatingPointConstant(info.Value));
                        else
                        {
                            if (!caseSensitive)
                                tokenValue = tokenValue.ToLowerFast();
                            resultStack.Push(new Variable(tokenValue));
                        }
                    }
                    break;
                case TokenType.LeftBracket:
                    operatorStack.Push(token);
                    break;
                case TokenType.RightBracket:
                    PopOperations(true, token);
                    break;
                case TokenType.ArgumentSeparator:
                    PopOperations(false, token);
                    parameterCount.Push(parameterCount.Pop() + 1);
                    break;
                case TokenType.Operation:
                    var operation1 = (char)token.Value;

                    while (operatorStack.Count > 0 && (operatorStack.Peek().TokenType == TokenType.Operation ||
                                                       operatorStack.Peek().TokenType == TokenType.Text))
                    {
                        var operation2Token = operatorStack.Peek();
                        var isFunctionOnTopOfStack = operation2Token.TokenType == TokenType.Text;

                        if (!isFunctionOnTopOfStack)
                        {
                            var operation2 = (char)operation2Token.Value;

                            if ((IsLeftAssociativeOperation(operation1) &&
                                 operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                (operationPrecedence[operation1] < operationPrecedence[operation2]))
                            {
                                operatorStack.Pop();
                                resultStack.Push(ConvertOperation(operation2Token));
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            operatorStack.Pop();
                            resultStack.Push(ConvertFunction(operation2Token));
                        }
                    }

                    operatorStack.Push(token);
                    break;
            }
        }

        PopOperations(false, null);

        VerifyResultStack();

        return resultStack.First();
    }

    private void PopOperations(bool untilLeftBracket, Token? currentToken)
    {
        if (untilLeftBracket && !currentToken.HasValue)
            throw new ArgumentNullException(nameof(currentToken), "If the parameter \"untilLeftBracket\" is set to true, " +
                                                                  "the parameter \"currentToken\" cannot be null.");

        while (operatorStack.Count > 0 && operatorStack.Peek().TokenType != TokenType.LeftBracket)
        {
            var token = operatorStack.Pop();

            switch (token.TokenType)
            {
                case TokenType.Operation:
                    resultStack.Push(ConvertOperation(token));
                    break;
                case TokenType.Text:
                    resultStack.Push(ConvertFunction(token));
                    break;
            }
        }

        if (untilLeftBracket)
        {
            if (operatorStack.Count > 0 && operatorStack.Peek().TokenType == TokenType.LeftBracket)
                operatorStack.Pop();
            else
                throw new ParseException("No matching left bracket found for the right " +
                                         $"bracket at position {currentToken!.Value.StartPosition}.");
        }
        else if (operatorStack.Count > 0 && operatorStack.Peek() is {TokenType: TokenType.LeftBracket }
                                         && currentToken is not { TokenType: TokenType.ArgumentSeparator })
            throw new ParseException("No matching right bracket found for the left " +
                                     $"bracket at position {operatorStack.Peek().StartPosition}.");
    }

    private Operation ConvertOperation(Token operationToken)
    {
        try
        {
            DataType dataType;
            Operation argument1;
            Operation argument2;

            switch ((char)operationToken.Value)
            {
                case '+':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new Addition(dataType, argument1, argument2);
                case '-':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new Subtraction(dataType, argument1, argument2);
                case '*':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new Multiplication(dataType, argument1, argument2);
                case '/':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    return new Division(DataType.FloatingPoint, argument1, argument2);
                case '%':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    return new Modulo(DataType.FloatingPoint, argument1, argument2);
                case '_':
                    argument1 = resultStack.Pop();
                    return new UnaryMinus(argument1.DataType, argument1);
                case '^':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    return new Exponentiation(DataType.FloatingPoint, argument1, argument2);
                case '&':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new And(dataType, argument1, argument2);
                case '|':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new Or(dataType, argument1, argument2);
                case '<':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new LessThan(dataType, argument1, argument2);
                case '≤':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new LessOrEqualThan(dataType, argument1, argument2);
                case '>':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new GreaterThan(dataType, argument1, argument2);
                case '≥':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new GreaterOrEqualThan(dataType, argument1, argument2);
                case '=':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new Equal(dataType, argument1, argument2);
                case '≠':
                    argument2 = resultStack.Pop();
                    argument1 = resultStack.Pop();
                    dataType = RequiredDataType(argument1, argument2);
                    return new NotEqual(dataType, argument1, argument2);
                default:
                    throw new ArgumentException($"Unknown operation \"{operationToken}\".", nameof(operationToken));
            }
        }
        catch (InvalidOperationException)
        {
            // If we encounter a Stack empty issue, this means there is a syntax issue in
            // the mathematical formula
            throw new ParseException($"There is a syntax issue for the operation \"{operationToken.Value}\" at position {operationToken.StartPosition}. " +
                                     "The number of arguments does not match with what is expected.");
        }
    }

    private Operation ConvertFunction(Token functionToken)
    {
        try
        {
            var functionName = ((string)functionToken.Value).ToLowerInvariant();

            if (!functionRegistry.ContainsFunctionName(functionName))
                throw new ArgumentException($"Unknown function \"{functionToken.Value}\".", nameof(functionToken));

            var functionInfo = functionRegistry.GetFunctionInfo(functionName);
            var numberOfParameters = parameterCount.Pop();

            if (!functionInfo.IsDynamicFunc)
                numberOfParameters = functionInfo.NumberOfParameters;

            var operations = new List<Operation>();
            for (var i = 0; i < numberOfParameters; i++)
                operations.Add(resultStack.Pop());
            operations.Reverse();

            return new Function(DataType.FloatingPoint, functionName, operations, functionInfo.IsIdempotent);

        }
        catch (InvalidOperationException)
        {
            // If we encounter a Stack empty issue, this means there is a syntax issue in
            // the mathematical formula
            throw new ParseException($"There is a syntax issue for the function \"{functionToken.Value}\" at position {functionToken.StartPosition}. " +
                                     "The number of arguments does not match with what is expected.");
        }
    }

    private void VerifyResultStack()
    {
        if (resultStack.Count <= 1) return;
        var operations = resultStack.ToArray();

        for (var i = 1; i < operations.Length; i++)
        {
            switch (operations[i])
            {
                case IntegerConstant integerConstant:
                    throw new ParseException($"Unexpected integer constant \"{integerConstant.Value}\" found.");
                case FloatingPointConstant floatConstant:
                    throw new ParseException($"Unexpected floating point constant \"{floatConstant.Value}\" found.");
            }
        }

        throw new ParseException("The syntax of the provided formula is not valid.");
    }

    private static bool IsLeftAssociativeOperation(char character)
    {
        return character is '*' or '+' or '-' or '/';
    }

    private static DataType RequiredDataType(Operation argument1, Operation argument2)
    {
        return (argument1.DataType == DataType.FloatingPoint
             || argument2.DataType == DataType.FloatingPoint)
                   ? DataType.FloatingPoint
                   : DataType.Integer;
    }
}