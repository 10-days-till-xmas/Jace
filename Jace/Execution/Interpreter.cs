using System;
using System.Collections.Generic;
using Jace.Operations;
using Jace.Util;
using JetBrains.Annotations;

namespace Jace.Execution;
[PublicAPI]
public sealed class Interpreter(bool caseSensitive) : IExecutor
{
    public bool CaseSensitive { get; } = caseSensitive;
    public Interpreter(): this(false) { }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry functionRegistry,
                                                                  IConstantRegistry constantRegistry)
    {
        functionRegistry = new ReadOnlyFunctionRegistry(functionRegistry);
        constantRegistry = new ReadOnlyConstantRegistry(constantRegistry);
        return CaseSensitive
                   ? variables => Execute(operation, functionRegistry, constantRegistry, variables)
                   : variables => Execute(operation, functionRegistry, constantRegistry, EngineUtil.ConvertVariableNamesToLowerCase(variables));
    }

    public double Execute(Operation operation,
                          IFunctionRegistry functionRegistry,
                          IConstantRegistry constantRegistry,
                          IDictionary<string, double>? variables = null)
    {
        return operation switch
        {
            null => throw new ArgumentNullException(nameof(operation)),
            Constant constant => constant.Evaluate(),
            Variable variable => variables?.TryGetValue(variable.Name, out var value)
                              ?? throw new VariableNotDefinedException("No variables were provided.")
                                     ? value
                                     : throw new VariableNotDefinedException(
                                           $"The variable \"{variable.Name}\" used is not defined."),
            Function function => function.Invoke(functionRegistry, _Execute),
            UnaryOperation unaryOperation => unaryOperation.Evaluate(_Execute(unaryOperation.Argument)),
            BinaryOperation binaryOperation => binaryOperation.Evaluate(_Execute(binaryOperation.Argument1),
                                                                        _Execute(binaryOperation.Argument2)),
            _ => throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".",
                                             nameof(operation))
        };

        double _Execute(Operation _operation)
        {
            return Execute(_operation, functionRegistry, constantRegistry, variables);
        }
    }
}