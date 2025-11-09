using System;
using System.Collections.Generic;
#if BENCHJACE
using Jace.Execution;
#else
using Yace.Execution;
#endif

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class ExecutorBenchmarks : YaceBenchmarkBase
{
    private static Interpreter Interpreter => new(true);
    private static DynamicCompiler DynamicCompiler => new(true);

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Unoptimized_Interpreter(ExpressionInfo expressionInfo) =>
        Interpreter.BuildFormula(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Optimized_Interpreter(ExpressionInfo expressionInfo) =>
        Interpreter.BuildFormula(expressionInfo.RootOperation_Optimized,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Unoptimized_DynamicCompiler(ExpressionInfo expressionInfo) =>
        DynamicCompiler.BuildFormula(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Func<IDictionary<string, double>, double> BuildFormula_Optimized_DynamicCompiler(ExpressionInfo expressionInfo) =>
        DynamicCompiler.BuildFormula(expressionInfo.RootOperation_Optimized,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
}
