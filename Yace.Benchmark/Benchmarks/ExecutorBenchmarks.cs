using System;
using System.Collections.Generic;
using Yace.Execution;


namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class ExecutorBenchmarks : YaceBenchmarkBase
{
    private static Interpreter Interpreter => new(true);
    private static Jace.Execution.Interpreter Interpreter_Jace => new(true);
    private static DynamicCompiler DynamicCompiler => new(true);
    private static Jace.Execution.DynamicCompiler DynamicCompiler_Jace => new(true);

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Unoptimized_Interpreter(ExpressionInfo expressionInfo) =>
        expressionInfo.Library switch
        {
            Library.Jace => Interpreter_Jace.BuildFormula(expressionInfo.RootOperation_Jace,
                expressionInfo.FunctionRegistry_Jace, expressionInfo.ConstantRegistry_Jace),
            Library.Yace => Interpreter.BuildFormula(expressionInfo.RootOperation,
                expressionInfo.FunctionRegistry, expressionInfo.ConstantRegistry),
            _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Optimized_Interpreter(ExpressionInfo expressionInfo) =>
        expressionInfo.Library switch
        {
            Library.Jace => Interpreter_Jace.BuildFormula(expressionInfo.RootOperation_Optimized_Jace,
                expressionInfo.FunctionRegistry_Jace, expressionInfo.ConstantRegistry_Jace),
            Library.Yace => Interpreter.BuildFormula(expressionInfo.RootOperation_Optimized,
                expressionInfo.FunctionRegistry, expressionInfo.ConstantRegistry),
            _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Unoptimized_DynamicCompiler(ExpressionInfo expressionInfo) =>
        expressionInfo.Library switch
        {
            Library.Jace => DynamicCompiler_Jace.BuildFormula(expressionInfo.RootOperation_Jace,
                expressionInfo.FunctionRegistry_Jace, expressionInfo.ConstantRegistry_Jace),
            Library.Yace => DynamicCompiler.BuildFormula(expressionInfo.RootOperation,
                expressionInfo.FunctionRegistry, expressionInfo.ConstantRegistry),
            _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Optimized_DynamicCompiler(ExpressionInfo expressionInfo) =>
        expressionInfo.Library switch
        {
            Library.Jace => DynamicCompiler_Jace.BuildFormula(expressionInfo.RootOperation_Optimized_Jace,
                expressionInfo.FunctionRegistry_Jace, expressionInfo.ConstantRegistry_Jace),
            Library.Yace => DynamicCompiler.BuildFormula(expressionInfo.RootOperation_Optimized,
                expressionInfo.FunctionRegistry, expressionInfo.ConstantRegistry),
            _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
        };
}
