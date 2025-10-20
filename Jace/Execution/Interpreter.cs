using System;
using System.Collections.Generic;
using System.Linq;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution;

public class Interpreter : IExecutor
{
    private readonly bool caseSensitive;

    public Interpreter(): this(false) { }

    public Interpreter(bool caseSensitive)
    {
        this.caseSensitive = caseSensitive;
    }
    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry functionRegistry,
                                                                  IConstantRegistry constantRegistry)
    {
        return caseSensitive
                   ? variables => Execute(operation, functionRegistry, constantRegistry, variables)
                   : variables =>
                   {
                       variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                       return Execute(operation, functionRegistry, constantRegistry, variables);
                   };
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
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (operation.GetType() == typeof(IntegerConstant))
        {
            var constant = (IntegerConstant)operation;
            return constant.Value;
        }

        if (operation.GetType() == typeof(FloatingPointConstant))
        {
            var constant = (FloatingPointConstant)operation;
            return constant.Value;
        }

        if (operation.GetType() == typeof(Variable))
        {
            var variable = (Variable)operation;

            return variables.TryGetValue(variable.Name, out var value)
                       ? value
                       : throw new VariableNotDefinedException($"The variable \"{variable.Name}\" used is not defined.");
        }

        if (operation.GetType() == typeof(Multiplication))
        {
            var multiplication = (Multiplication)operation;
            return Execute(multiplication.Argument1, functionRegistry, constantRegistry, variables) * Execute(multiplication.Argument2, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(Addition))
        {
            var addition = (Addition)operation;
            return Execute(addition.Argument1, functionRegistry, constantRegistry, variables) + Execute(addition.Argument2, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(Subtraction))
        {
            var addition = (Subtraction)operation;
            return Execute(addition.Argument1, functionRegistry, constantRegistry, variables) - Execute(addition.Argument2, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(Division))
        {
            var division = (Division)operation;
            return Execute(division.Argument1, functionRegistry, constantRegistry, variables) / Execute(division.Argument2, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(Modulo))
        {
            var division = (Modulo)operation;
            return Execute(division.Argument1, functionRegistry, constantRegistry, variables) % Execute(division.Argument2, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(Exponentiation))
        {
            var exponentiation = (Exponentiation)operation;
            return Math.Pow(Execute(exponentiation.Argument1, functionRegistry, constantRegistry, variables), Execute(exponentiation.Argument2, functionRegistry, constantRegistry, variables));
        }

        if (operation.GetType() == typeof(UnaryMinus))
        {
            var unaryMinus = (UnaryMinus)operation;
            return -Execute(unaryMinus.Argument, functionRegistry, constantRegistry, variables);
        }

        if (operation.GetType() == typeof(And))
        {
            var and = (And)operation;
            var operation1 = Execute(and.Argument1, functionRegistry, constantRegistry, variables) != 0;
            var operation2 = Execute(and.Argument2, functionRegistry, constantRegistry, variables) != 0;

            return (operation1 && operation2) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(Or))
        {
            var or = (Or)operation;
            var operation1 = Execute(or.Argument1, functionRegistry, constantRegistry, variables) != 0;
            var operation2 = Execute(or.Argument2, functionRegistry, constantRegistry, variables) != 0;

            return (operation1 || operation2) ? 1.0 : 0.0;
        }

        if(operation.GetType() == typeof(LessThan))
        {
            var lessThan = (LessThan)operation;
            return (Execute(lessThan.Argument1, functionRegistry, constantRegistry, variables) < Execute(lessThan.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(LessOrEqualThan))
        {
            var lessOrEqualThan = (LessOrEqualThan)operation;
            return (Execute(lessOrEqualThan.Argument1, functionRegistry, constantRegistry, variables) <= Execute(lessOrEqualThan.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(GreaterThan))
        {
            var greaterThan = (GreaterThan)operation;
            return (Execute(greaterThan.Argument1, functionRegistry, constantRegistry, variables) > Execute(greaterThan.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(GreaterOrEqualThan))
        {
            var greaterOrEqualThan = (GreaterOrEqualThan)operation;
            return (Execute(greaterOrEqualThan.Argument1, functionRegistry, constantRegistry, variables) >= Execute(greaterOrEqualThan.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(Equal))
        {
            var equal = (Equal)operation;
            return (Execute(equal.Argument1, functionRegistry, constantRegistry, variables) == Execute(equal.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(NotEqual))
        {
            var notEqual = (NotEqual)operation;
            return (Execute(notEqual.Argument1, functionRegistry, constantRegistry, variables) != Execute(notEqual.Argument2, functionRegistry, constantRegistry, variables)) ? 1.0 : 0.0;
        }

        if (operation.GetType() == typeof(Function))
        {
            var function = (Function)operation;

            var functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);

            var arguments = new double[functionInfo.IsDynamicFunc ? function.Arguments.Count : functionInfo.NumberOfParameters];
            for (var i = 0; i < arguments.Length; i++)
                arguments[i] = Execute(function.Arguments[i], functionRegistry, constantRegistry, variables);

            return Invoke(functionInfo.Function, arguments);
        }

        throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
    }

    private double Invoke(Delegate function, double[] arguments)
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
            _ => (double)function.DynamicInvoke((args.Select(s => (object)s)).ToArray())
        };
    }
}