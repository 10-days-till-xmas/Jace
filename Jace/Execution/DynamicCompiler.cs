using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution;

public class DynamicCompiler : IExecutor
{
    // The lower func reside in mscorlib, the higher ones in another assembly.
    // This is an easy cross-platform way to have this AssemblyQualifiedName.
    private static readonly string FuncAssemblyQualifiedName =
        typeof(Func<double, double, double, double, double, double, double, double, double, double>).GetTypeInfo().Assembly.FullName;

    private readonly bool caseSensitive;

    public DynamicCompiler(): this(false) { }
    public DynamicCompiler(bool caseSensitive)
    {
        this.caseSensitive = caseSensitive;
    }

    public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        return Execute(operation, functionRegistry, constantRegistry, new Dictionary<string, double>());
    }

    public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
                          IDictionary<string, double> variables)
    {
        return BuildFormula(operation, functionRegistry, constantRegistry)(variables);
    }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        var func = BuildFormulaInternal(operation, functionRegistry);
        return caseSensitive
                   ? variables => func(new FormulaContext(variables, functionRegistry, constantRegistry))
                   : variables =>
                   {
                       variables = EngineUtil.ConvertVariableNamesToLowerCase(variables);
                       var context = new FormulaContext(variables, functionRegistry, constantRegistry);
                       return func(context);
                   };
    }

    private Func<FormulaContext, double> BuildFormulaInternal(Operation operation,
                                                              IFunctionRegistry functionRegistry)
    {
        var contextParameter = Expression.Parameter(typeof(FormulaContext), "context");

        return Expression.Lambda<Func<FormulaContext, double>>(GenerateMethodBody(operation, contextParameter, functionRegistry),
                                                               contextParameter)
                         .Compile();
    }



    private Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter,
                                          IFunctionRegistry functionRegistry)
    {
        if (operation == null)
            throw new ArgumentNullException(nameof(operation));

        if (operation.GetType() == typeof(IntegerConstant))
        {
            var constant = (IntegerConstant)operation;

            double value = constant.Value;
            return Expression.Constant(value, typeof(double));
        }

        if (operation.GetType() == typeof(FloatingPointConstant))
        {
            var constant = (FloatingPointConstant)operation;

            return Expression.Constant(constant.Value, typeof(double));
        }

        if (operation.GetType() == typeof(Variable))
        {
            var variable = (Variable)operation;

            var getVariableValueOrThrow = PrecompiledMethods.GetVariableValueOrThrow;
            return Expression.Call(null,
                                   getVariableValueOrThrow.GetMethodInfo(),
                                   Expression.Constant(variable.Name),
                                   contextParameter);
        }

        if (operation.GetType() == typeof(Multiplication))
        {
            var multiplication = (Multiplication)operation;
            var argument1 = GenerateMethodBody(multiplication.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(multiplication.Argument2, contextParameter, functionRegistry);

            return Expression.Multiply(argument1, argument2);
        }

        if (operation.GetType() == typeof(Addition))
        {
            var addition = (Addition)operation;
            var argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

            return Expression.Add(argument1, argument2);
        }

        if (operation.GetType() == typeof(Subtraction))
        {
            var addition = (Subtraction)operation;
            var argument1 = GenerateMethodBody(addition.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(addition.Argument2, contextParameter, functionRegistry);

            return Expression.Subtract(argument1, argument2);
        }

        if (operation.GetType() == typeof(Division))
        {
            var division = (Division)operation;
            var dividend = GenerateMethodBody(division.Argument1, contextParameter, functionRegistry);
            var divisor = GenerateMethodBody(division.Argument2, contextParameter, functionRegistry);

            return Expression.Divide(dividend, divisor);
        }

        if (operation.GetType() == typeof(Modulo))
        {
            var modulo = (Modulo)operation;
            var dividend = GenerateMethodBody(modulo.Argument1, contextParameter, functionRegistry);
            var divisor = GenerateMethodBody(modulo.Argument2, contextParameter, functionRegistry);

            return Expression.Modulo(dividend, divisor);
        }

        if (operation.GetType() == typeof(Exponentiation))
        {
            var exponentiation = (Exponentiation)operation;
            var @base = GenerateMethodBody(exponentiation.Argument1, contextParameter, functionRegistry);
            var exponent = GenerateMethodBody(exponentiation.Argument2, contextParameter, functionRegistry);

            return Expression.Call(null, typeof(Math).GetRuntimeMethod("Pow", [typeof(double), typeof(double)]), @base, exponent);
        }

        if (operation.GetType() == typeof(UnaryMinus))
        {
            var unaryMinus = (UnaryMinus)operation;
            var argument = GenerateMethodBody(unaryMinus.Argument, contextParameter, functionRegistry);
            return Expression.Negate(argument);
        }

        if (operation.GetType() == typeof(And))
        {
            var and = (And)operation;
            Expression argument1 = Expression.NotEqual(GenerateMethodBody(and.Argument1, contextParameter, functionRegistry), Expression.Constant(0.0));
            Expression argument2 = Expression.NotEqual(GenerateMethodBody(and.Argument2, contextParameter, functionRegistry), Expression.Constant(0.0));

            return Expression.Condition(Expression.And(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(Or))
        {
            var and = (Or)operation;
            Expression argument1 = Expression.NotEqual(GenerateMethodBody(and.Argument1, contextParameter, functionRegistry), Expression.Constant(0.0));
            Expression argument2 = Expression.NotEqual(GenerateMethodBody(and.Argument2, contextParameter, functionRegistry), Expression.Constant(0.0));

            return Expression.Condition(Expression.Or(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(LessThan))
        {
            var lessThan = (LessThan)operation;
            var argument1 = GenerateMethodBody(lessThan.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(lessThan.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.LessThan(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(LessOrEqualThan))
        {
            var lessOrEqualThan = (LessOrEqualThan)operation;
            var argument1 = GenerateMethodBody(lessOrEqualThan.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(lessOrEqualThan.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.LessThanOrEqual(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(GreaterThan))
        {
            var greaterThan = (GreaterThan)operation;
            var argument1 = GenerateMethodBody(greaterThan.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(greaterThan.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.GreaterThan(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(GreaterOrEqualThan))
        {
            var greaterOrEqualThan = (GreaterOrEqualThan)operation;
            var argument1 = GenerateMethodBody(greaterOrEqualThan.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(greaterOrEqualThan.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.GreaterThanOrEqual(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(Equal))
        {
            var equal = (Equal)operation;
            var argument1 = GenerateMethodBody(equal.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(equal.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.Equal(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(NotEqual))
        {
            var notEqual = (NotEqual)operation;
            var argument1 = GenerateMethodBody(notEqual.Argument1, contextParameter, functionRegistry);
            var argument2 = GenerateMethodBody(notEqual.Argument2, contextParameter, functionRegistry);

            return Expression.Condition(Expression.NotEqual(argument1, argument2),
                                        Expression.Constant(1.0),
                                        Expression.Constant(0.0));
        }

        if (operation.GetType() == typeof(Function))
        {
            var function = (Function)operation;

            var functionInfo = functionRegistry.GetFunctionInfo(function.FunctionName);
            Type funcType;
            Type[] parameterTypes;
            Expression[] arguments;

            if (functionInfo.IsDynamicFunc)
            {
                funcType = typeof(DynamicFunc<double, double>);
                parameterTypes = [typeof(double[])];


                var arrayArguments = new Expression[function.Arguments.Count];
                for (var i = 0; i < function.Arguments.Count; i++)
                    arrayArguments[i] = GenerateMethodBody(function.Arguments[i], contextParameter, functionRegistry);

                arguments = new Expression[1];
                arguments[0] = Expression.NewArrayInit(typeof(double), arrayArguments);
            }
            else
            {
                funcType = GetFuncType(functionInfo.NumberOfParameters);
                parameterTypes = (from i in Enumerable.Range(0, functionInfo.NumberOfParameters)
                                  select typeof(double)).ToArray();

                arguments = new Expression[functionInfo.NumberOfParameters];
                for (var i = 0; i < functionInfo.NumberOfParameters; i++)
                    arguments[i] = GenerateMethodBody(function.Arguments[i], contextParameter, functionRegistry);
            }

            Expression getFunctionRegistry = Expression.Property(contextParameter, "FunctionRegistry");

            var functionInfoVariable = Expression.Variable(typeof(FunctionInfo));

            Expression funcInstance;
            if (!functionInfo.IsOverWritable)
            {
                funcInstance = Expression.Convert(
                                                  Expression.Property(
                                                                      Expression.Call(
                                                                           getFunctionRegistry,
                                                                           typeof(IFunctionRegistry).GetRuntimeMethod("GetFunctionInfo",
                                                                               [typeof(string)]),
                                                                           Expression.Constant(function.FunctionName)),
                                                                      "Function"),
                                                  funcType);
            }
            else
                funcInstance = Expression.Constant(functionInfo.Function, funcType);

            return Expression.Call(
                                   funcInstance,
                                   funcType.GetRuntimeMethod("Invoke", parameterTypes),
                                   arguments);
        }

        throw new ArgumentException($"Unsupported operation \"{operation.GetType().FullName}\".", nameof(operation));
    }

    private Type GetFuncType(int numberOfParameters)
    {
        var funcTypeName = numberOfParameters < 9
                               ? $"System.Func`{numberOfParameters + 1}"
                               : $"System.Func`{numberOfParameters + 1}, {FuncAssemblyQualifiedName}";
        var funcType = Type.GetType(funcTypeName);

        var typeArguments = Enumerable.Repeat(typeof(double), numberOfParameters + 1)
                                      .ToArray();

        return funcType?.MakeGenericType(typeArguments);
    }

    private static class PrecompiledMethods
    {
        public static double GetVariableValueOrThrow(string variableName, FormulaContext context)
        {
            if (context.Variables.TryGetValue(variableName, out var result))
                return result;
            if (context.ConstantRegistry.IsConstantName(variableName))
                return context.ConstantRegistry.GetConstantInfo(variableName).Value;
            throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
        }
    }
}