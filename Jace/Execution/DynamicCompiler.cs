using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Jace.Operations;
using Jace.Util;

namespace Jace.Execution;

public sealed class DynamicCompiler(bool caseSensitive = false) : IExecutor
{
    public double Execute(Operation operation, IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry, 
        IDictionary<string, double>? variables = null)
    {
        variables ??= new Dictionary<string, double>();
        return BuildFormula(operation, functionRegistry, constantRegistry)(variables);
    }

    public Func<IDictionary<string, double>, double> BuildFormula(Operation operation,
        IFunctionRegistry functionRegistry, IConstantRegistry constantRegistry)
    {
        var func = BuildFormulaInternal(operation, functionRegistry);
        return variables 
            => func(new FormulaContext(
                caseSensitive 
                    ? variables
                    : EngineUtil.ConvertVariableNamesToLowerCase(variables), 
                functionRegistry, 
                constantRegistry));
    }

    private Func<FormulaContext, double> BuildFormulaInternal(Operation operation, 
        IFunctionRegistry functionRegistry)
    {
        var contextParameter = Expression.Parameter(typeof(FormulaContext), "context");

        Expression.Label(typeof(double));

        var lambda = Expression.Lambda<Func<FormulaContext, double>>(
            GenerateMethodBody(operation, contextParameter, functionRegistry),
            contextParameter
        );
        return lambda.Compile();
    }

    public Expression GenerateMethodBody(Operation operation, ParameterExpression contextParameter,
        IFunctionRegistry functionRegistry)
    {
        return operation.GenerateMethodBody(this, contextParameter, functionRegistry);
    }

    public static class PrecompiledMethods
    {
        public static double GetVariableValueOrThrow(string variableName, FormulaContext context)
        {
            if (context.Variables.TryGetValue(variableName, out double result))
                return result;
            if (context.ConstantRegistry.TryGetConstantInfo(variableName, out var info))
                return info!.Value;
            throw new VariableNotDefinedException($"The variable \"{variableName}\" used is not defined.");
        }
    }
}