using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Yace.Operations;
using Yace.Util;

namespace Yace.Execution;
[PublicAPI]
public sealed class Interpreter(bool caseSensitive) : IExecutor
{
    public bool CaseSensitive { get; } = caseSensitive;
    public Interpreter(): this(false) { }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry? functionRegistry,
                                                                  IConstantRegistry? constantRegistry)
    {
        functionRegistry = new ReadOnlyFunctionRegistry(functionRegistry ?? new FunctionRegistry(CaseSensitive));
        constantRegistry = new ReadOnlyConstantRegistry(constantRegistry ?? new ConstantRegistry(CaseSensitive));
        return CaseSensitive
                   ? variables => Execute(operation, functionRegistry, constantRegistry, variables)
                   : variables => Execute(operation, functionRegistry, constantRegistry,
                       new Dictionary<string, double>(variables, StringComparer.OrdinalIgnoreCase));
    }

    public double Execute_old(Operation operation,
                          IFunctionRegistry? functionRegistry,
                          IConstantRegistry? constantRegistry,
                          IDictionary<string, double>? variables = null)
    {
        // TODO: refactor to avoid recursion overhead
        return operation switch
        {
            null => throw new ArgumentNullException(nameof(operation)),
            Constant constant => constant.DoubleValue,
            Variable variable => variables?.TryGetValue(variable.Name, out var value)
                              ?? throw new VariableNotDefinedException("No variables were provided.")
                                     ? value
                                     : throw new VariableNotDefinedException(
                                           $"The variable \"{variable.Name}\" used is not defined."),
            Function function => function.Invoke(functionRegistry ?? throw new InvalidOperationException("functionRegistry is null."), _Execute),
            UnaryOperation unaryOperation => unaryOperation.Evaluate(_Execute(unaryOperation.Argument)),
            BinaryOperation binaryOperation => binaryOperation.Evaluate(_Execute(binaryOperation.Argument1),
                                                                        _Execute(binaryOperation.Argument2)),
            _ => throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".",
                                             nameof(operation))
        };

        double _Execute(Operation _operation)
        {
            return Execute_old(_operation, functionRegistry, constantRegistry, variables);
        }
    }
    public double Execute(Operation operation,
                      IFunctionRegistry? functionRegistry,
                      IConstantRegistry? constantRegistry,
                      IDictionary<string, double>? variables = null)
    {
        #if NETSTANDARD2_0
        if (operation is null)
            throw new ArgumentNullException(nameof(operation));
        #else
        ArgumentNullException.ThrowIfNull(operation);
        #endif
        // try using ArrayPool instead?
        var stack = new Stack<(Operation Op, bool IsEvaluated)>(10); // maxSize 3 or 8 (depth of tree
        var valueStack = new Stack<double>(10); // max size 2 or 3 (basically horizontal width of tree)
        var maxSize = 0;
        stack.Push((operation, false));

        while (stack.Count > 0)
        {
            if (valueStack.Count > maxSize)
                maxSize = valueStack.Count;
            switch (stack.Pop())
            {
                case (null, _): // this probably isn't necessary since the AstBuilder shouldn't produce null operations
                    throw new ArgumentNullException(nameof(operation));
                case (Constant c, _):
                    valueStack.Push(c.DoubleValue);
                    break;
                case (Variable v, _):
                    if (variables == null)
                        throw new VariableNotDefinedException("No variables were provided.");
                    if (!variables.TryGetValue(v.Name, out var varValue))
                        throw new VariableNotDefinedException($"The variable \"{v.Name}\" used is not defined.");
                    valueStack.Push(varValue);
                    break;
                case (UnaryOperation u, false):
                    stack.Push((u, true));
                    stack.Push((u.Argument, false));
                    break;

                case (BinaryOperation b, false):
                    stack.Push((b, true));
                    stack.Push((b.Argument2, false));
                    stack.Push((b.Argument1, false));
                    break;

                case (Function f, false):
                    if (functionRegistry == null)
                        throw new InvalidOperationException("functionRegistry is null.");
                    stack.Push((f, true));
                    for (var i = f.Arguments.Count - 1; i >= 0; i--) // Avoid Linq.Reverse
                        stack.Push((f.Arguments[i], false));
                    break;

                case (UnaryOperation marker, true):
                    var unaryArg = valueStack.Pop();
                    valueStack.Push(marker.Evaluate(unaryArg));
                    break;

                case (BinaryOperation marker, true):
                    var right = valueStack.Pop();
                    var left = valueStack.Pop();
                    valueStack.Push(marker.Evaluate(left, right));
                    break;

                case (Function marker, true):
                    var func = functionRegistry.GetFunctionInfo(marker.FunctionName).Function;
                    #if !NETSTANDARD2_0
                    static double InvokeFunction(Stack<double> valueStack, Delegate function, int argCount)
                    { // prevent stackOverflow from stackalloc in loop
                        Span<double> arr = stackalloc double[argCount];
                        for (var i = argCount - 1; i >= 0; i--)
                            arr[i] = valueStack.Pop();
                        return FuncUtil.Invoke(function, arr);
                    }
                    var res = InvokeFunction(valueStack, func, marker.Arguments.Count);
                    #else
                    var argValues = valueStack.PopMany(marker.Arguments.Count);
                    var res = FuncUtil.Invoke(func, argValues);
                    #endif
                    valueStack.Push(res);
                    break;

                case { } val:
                    throw new ArgumentException($"Unsupported operation \"{val.GetType().FullName}\".", nameof(operation));
            }
        }
        Console.WriteLine(maxSize);
        return valueStack.Pop();
    }
}


