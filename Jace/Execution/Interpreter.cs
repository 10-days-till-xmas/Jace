using System;
using System.Collections.Generic;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution;

public sealed class Interpreter(bool caseSensitive) : IExecutor
{
    public Interpreter(): this(false) { }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry functionRegistry,
                                                                  IConstantRegistry constantRegistry)
    {
        return caseSensitive
                   ? variables => Execute(operation, functionRegistry, constantRegistry, variables)
                   : variables => Execute(operation, functionRegistry, constantRegistry, EngineUtil.ConvertVariableNamesToLowerCase(variables));
    }

    public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        return Execute(operation, functionRegistry, constantRegistry, new Dictionary<string, double>());
    }

    public double Execute(Operation operation,
                          IFunctionRegistry functionRegistry,
                          IConstantRegistry constantRegistry,
                          IDictionary<string, double> variables)
    {
        switch (operation)
        {
            case null:
                throw new ArgumentNullException(nameof(operation));
            case IntegerConstant integerConstant:
                return integerConstant.Value;
            case FloatingPointConstant floatingPointConstant:
                return floatingPointConstant.Value;
            case Variable variable:
                return variables.TryGetValue(variable.Name, out var value)
                           ? value
                           : throw new VariableNotDefinedException($"The variable \"{variable.Name}\" used is not defined.");
            case Function function:
                var functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

                var arguments = new double[functionInfo.IsDynamicFunc ? function.Arguments.Count : functionInfo.NumberOfParameters];
                for (var i = 0; i < arguments.Length; i++)
                    arguments[i] = Execute(function.Arguments[i], functionRegistry, constantRegistry, variables);

                return Invoke(functionInfo.Function, arguments);
            case UnaryOperation unaryOperation:
                var arg = Execute(unaryOperation.Argument, functionRegistry, constantRegistry, variables);
                return unaryOperation switch
                {
                    UnaryMinus => -arg,
                    _ => throw new ArgumentException($"Unsupported unary operation \"{unaryOperation}\".", nameof(operation))
                };
            case BinaryOperation binaryOperation:
                var arg1 = Execute(binaryOperation.Argument1, functionRegistry, constantRegistry, variables);
                var arg2 = Execute(binaryOperation.Argument2, functionRegistry, constantRegistry, variables);
                return binaryOperation switch
                {
                    Addition => arg1 + arg2,
                    Subtraction => arg1 - arg2,
                    Multiplication => arg1 * arg2,
                    Division => arg1 / arg2,
                    Modulo => arg1 % arg2,
                    Exponentiation => Math.Pow(arg1, arg2),
                    And => (arg1.AsBool() && arg2.AsBool()).AsDouble(),
                    Or => (arg1.AsBool() || arg2.AsBool()).AsDouble(),
                    Equal => arg1.Equals(arg2).AsDouble(),
                    NotEqual => (!arg1.Equals(arg2)).AsDouble(),
                    LessThan => (arg1 < arg2).AsDouble(),
                    LessOrEqualThan => (arg1 <= arg2).AsDouble(),
                    GreaterThan => (arg1 > arg2).AsDouble(),
                    GreaterOrEqualThan => (arg1 >= arg2).AsDouble(),
                    _ => throw new ArgumentException($"Unsupported binary operation \"{binaryOperation}\".", nameof(operation))
                };
            default:
                throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
        }
    }

    private static double Invoke(Delegate function, double[] arguments)
    {
        var args = arguments;
        return function switch
        {
            // DynamicInvoke is slow, so we first try to convert it to a Func
            Func<double> func0
                => func0.Invoke(),
            Func<double, double> func1
                => func1.Invoke(args[0]),
            Func<double, double, double> func2
                => func2.Invoke(args[0], args[1]),
            Func<double, double, double, double> func3
                => func3.Invoke(args[0], args[1], args[2]),
            Func<double, double, double, double, double> func4
                => func4.Invoke(args[0], args[1], args[2], args[3]),
            Func<double, double, double, double, double, double> func5
                => func5.Invoke(args[0], args[1], args[2], args[3], args[4]),
            Func<double, double, double, double, double, double, double> func6
                => func6.Invoke(args[0], args[1], args[2], args[3], args[4], args[5]),
            Func<double, double, double, double, double, double, double, double> func7
                => func7.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6]),
            Func<double, double, double, double, double, double, double, double, double> func8 =>
                func8.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]),
            Func<double, double, double, double, double, double, double, double, double, double> func9 =>
                func9.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]),
            Func<double, double, double, double, double, double, double, double, double, double, double> func10 =>
                func10.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double> func11
                => func11.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double> func12
                => func12.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double> func13
                => func13.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func14
                => func14.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func15
                => func15.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14]),
            Func<double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double, double> func16
                => func16.Invoke(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10], args[11], args[12], args[13], args[14], args[15]),
            DynamicFunc<double, double> dynamicFunc => dynamicFunc.Invoke(args),
            _ => (double)function.DynamicInvoke(args)!
        };
    }
}