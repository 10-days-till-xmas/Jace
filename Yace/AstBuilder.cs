using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yace.Execution;
using Yace.Operations;
using Yace.Tokenizer;

namespace Yace;

public sealed class AstBuilder(
    IFunctionRegistry? functionRegistry,
    bool caseSensitive,
    IConstantRegistry? compiledConstants = null)
    : IUsesText
{
    private readonly IFunctionRegistry functionRegistry = functionRegistry ?? new FunctionRegistry(caseSensitive);
    private readonly IConstantRegistry localConstantRegistry = compiledConstants ?? new ConstantRegistry(caseSensitive);
    public bool CaseSensitive { get; } = caseSensitive;

    private readonly Dictionary<char, int> operationPrecedence = new()
    {
        { '(', 0 }, { '&', 1 }, { '|', 1 }, { '<', 2 }, { '>', 2 }, { '≤', 2 }, { '≥', 2 }, { '≠', 2 },
        { '=', 2 }, { '+', 3 }, { '-', 3 }, { '*', 4 }, { '/', 4 }, { '%', 4 }, { '^', 5 }, { '_', 6 }
    };
    private readonly Stack<Operation> resultStack = new();
    private readonly Stack<Token> operatorStack = new();
    private readonly Stack<int> parameterCount = new();

    public Operation Build(IEnumerable<Token> tokens)
    {
        resultStack.Clear();
        operatorStack.Clear();

        parameterCount.Clear();

        foreach (var token in tokens)
        {
            switch (token.TokenType)
            {
                case TokenType.Integer:
                    resultStack.Push(new IntegerConstant(token.IntValue));
                    break;
                case TokenType.FloatingPoint:
                    resultStack.Push(new FloatingPointConstant(token.FloatValue));
                    break;
                case TokenType.Text:
                    var tokenValue = token.StringValue;
                    if (functionRegistry.ContainsName(tokenValue))
                    {
                        operatorStack.Push(token);
                        parameterCount.Push(1);
                    }
                    else if (localConstantRegistry.TryGetConstantInfo(tokenValue, out var info))
                        resultStack.Push(new FloatingPointConstant(info.Value));
                    else
                        resultStack.Push(new Variable(tokenValue));
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
                    var operation1 = token.CharValue;

                    while (operatorStack.Count > 0 && (operatorStack.Peek().TokenType == TokenType.Operation ||
                                                       operatorStack.Peek().TokenType == TokenType.Text))
                    {
                        var operation2Token = operatorStack.Peek();
                        var isFunctionOnTopOfStack = operation2Token.TokenType == TokenType.Text;

                        if (!isFunctionOnTopOfStack)
                        {
                            var operation2 = operation2Token.CharValue;

                            if ((IsLeftAssociativeOperation(operation1) &&
                                 operationPrecedence[operation1] <= operationPrecedence[operation2]) ||
                                (operationPrecedence[operation1] < operationPrecedence[operation2]))
                            {
                                operatorStack.Pop();
                                resultStack.Push(ConvertOperation(operation2Token));
                            }
                            else break;
                        }
                        else
                        {
                            operatorStack.Pop();
                            resultStack.Push(ConvertFunction(operation2Token));
                        }
                    }

                    operatorStack.Push(token);
                    break;
                default: // Never happens due to Token implementation
                    throw new ArgumentOutOfRangeException(nameof(tokens), $"Unknown token type \"{token.TokenType}\".");
            }
        }

        PopOperations(false, null);

        VerifyResultStack();

        return resultStack.Peek();
    }

    private void PopOperations(bool untilLeftBracket, Token? currentToken)
    {
        if (untilLeftBracket && !currentToken.HasValue)
            throw new ArgumentNullException(nameof(currentToken), "If the parameter \"untilLeftBracket\" is set to true, " +
                                                                  "the parameter \"currentToken\" cannot be null.");

        while (operatorStack.Count > 0 && operatorStack.Peek().TokenType != TokenType.LeftBracket)
        {
            var token = operatorStack.Pop();

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
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
        if (operationToken.TokenType != TokenType.Operation)
            throw new ArgumentException("The provided token is not an operation token.", nameof(operationToken));
        try
        {
            if (operationToken.CharValue == '_') // unary minus
            {
                var arg = resultStack.Pop();
                return new UnaryMinus(arg.DataType, arg);
            }
            var argument2 = resultStack.Pop();
            var argument1 = resultStack.Pop();
            var dataType = RequiredDataType(argument1, argument2);
            return operationToken.CharValue switch
            {
                '+' => new Addition(dataType, argument1, argument2),
                '-' => new Subtraction(dataType, argument1, argument2),
                '*' => new Multiplication(dataType, argument1, argument2),
                '/' => new Division(DataType.FloatingPoint, argument1, argument2),
                '%' => new Modulo(DataType.FloatingPoint, argument1, argument2), // could return int actually...
                '^' => new Exponentiation(DataType.FloatingPoint, argument1, argument2), // could return int actually...
                '&' => new And(dataType, argument1, argument2),
                '|' => new Or(dataType, argument1, argument2),
                '<' => new LessThan(dataType, argument1, argument2),
                '≤' => new LessOrEqualThan(dataType, argument1, argument2),
                '>' => new GreaterThan(dataType, argument1, argument2),
                '≥' => new GreaterOrEqualThan(dataType, argument1, argument2),
                '=' => new Equal(dataType, argument1, argument2),
                '≠' => new NotEqual(dataType, argument1, argument2),
                _   => throw new ArgumentException($"Unknown operation \"{operationToken}\".", nameof(operationToken))
            };
        }
        catch (InvalidOperationException)
        {
            // If we encounter a Stack empty issue, this means there is a syntax issue in
            // the mathematical formula
            throw new ParseException($"There is a syntax issue for the operation \"{operationToken.CharValue}\" at position {operationToken.StartPosition}. " +
                                     "The number of arguments does not match with what is expected.");
        }
    }

    private Function ConvertFunction(Token functionToken)
    {
        if (functionToken.TokenType != TokenType.Text)
            throw new ArgumentException("The provided token is not a function token.", nameof(functionToken));
        try
        {
            var functionName = functionToken.StringValue;

            if (!functionRegistry.ContainsName(functionName))
                throw new ArgumentException($"Unknown function \"{functionToken.StringValue}\".", nameof(functionToken));

            var functionInfo = functionRegistry.GetInfo(functionName);
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
            throw new ParseException($"There is a syntax issue for the function \"{functionToken.StringValue}\" at position {functionToken.StartPosition}. " +
                                     "The number of arguments does not match with what is expected.");
        }
    }

    private void VerifyResultStack()
    {
        if (resultStack.Count <= 1) return;
        var operations = resultStack.ToArray();
        // wait this doesn't make sense
        foreach (var operation in operations)
            switch (operation)
            {
                case IntegerConstant integerConstant:
                    throw new ParseException($"Unexpected integer constant \"{integerConstant.Value}\" found.");
                case FloatingPointConstant floatConstant:
                    throw new ParseException($"Unexpected floating point constant \"{floatConstant.Value}\" found.");
            }
        throw new ParseException("The syntax of the provided formula is not valid.");
    }

    private static bool IsLeftAssociativeOperation(char character)
    {
        return character is '*' or '+' or '-' or '/';
    }
    [Pure]
    private static DataType RequiredDataType(Operation argument1, Operation argument2)
    {
        return (argument1.DataType == DataType.FloatingPoint
             || argument2.DataType == DataType.FloatingPoint)
                   ? DataType.FloatingPoint
                   : DataType.Integer;
    }
}
