using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jace.Operations;
using Jace.Util;
using JetBrains.Annotations;

namespace Jace.Execution;

[PublicAPI]
public sealed class DynamicCompiler(bool caseSensitive) : IExecutor
{
    public bool CaseSensitive { get; } = caseSensitive;

    // The lower func reside in mscorlib, the higher ones in another assembly.
    // This is an easy cross-platform way to have this AssemblyQualifiedName.
    private static readonly string FuncAssemblyQualifiedName =
        typeof(Func<double, double, double, double, double, double, double, double, double, double>).GetTypeInfo().Assembly.FullName;

    public DynamicCompiler(): this(false) { }

    public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry,
                          IDictionary<string, double>? variables = null)
    {
        variables ??= new Dictionary<string, double>();
        return BuildFormula(operation, functionRegistry, constantRegistry)(variables);
    }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
                                                                  IFunctionRegistry functionRegistry,
                                                                  IConstantRegistry constantRegistry)
    {
        var func = BuildFormulaInternal(operation, functionRegistry);
        return CaseSensitive
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
        switch (operation)
        {
            case null:
                throw new ArgumentNullException(nameof(operation));
            case Constant constant:
            {
                return Expression.Constant(constant.Evaluate());
            }
            case Variable variable1:
            {
                var getVariableValueOrThrow = PrecompiledMethods.GetVariableValueOrThrow;
                return Expression.Call(null,
                                       getVariableValueOrThrow.GetMethodInfo(),
                                       Expression.Constant(variable1.Name),
                                       contextParameter);
            }
            case UnaryOperation unaryOperation:
                var arg = GenerateMethodBody(unaryOperation.Argument, contextParameter, functionRegistry);
                return unaryOperation.GenerateExpression(arg);
            case BinaryOperation binaryOperation:
                var arg1 = GenerateMethodBody(binaryOperation.Argument1, contextParameter, functionRegistry);
                var arg2 = GenerateMethodBody(binaryOperation.Argument2, contextParameter, functionRegistry);
                return binaryOperation.GenerateExpression(arg1, arg2);
            case Function function:
                return function.AsExpression(functionRegistry, contextParameter,
                                            op => GenerateMethodBody(op, contextParameter, functionRegistry));
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
            if (context.Variables.TryGetValue(variableName, out var variable))
                return variable;
            if (context.ConstantRegistry.TryGetConstantInfo(variableName, out var info))
                return info.Value;
            throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
        }
    }
}