using System;
using Yace.Benchmark.Benchmarks;

namespace Yace.Benchmark;

public static class Profiler
{
    private const int _iterations = 1000;
    private static readonly AstBuilderBenchmarks _astBuilderBenchmarks = new();
    private static readonly ExecutorBenchmarks _executorBenchmarks = new();
    private static readonly OptimizerBenchmarks _optimizerBenchmarks = new();
    private static readonly TokenReaderBenchmarks _tokenReaderBenchmarks = new();
    private static readonly FunctionExecutionBenchmarks _functionExecutionBenchmarks = new();
    public static void RunClean()
    {
        for (var i = 0; i < _iterations; i++)
            ProfileAstBuilder();
        Console.WriteLine("AST Builder profiling complete.");
        for (var i = 0; i < _iterations; i++)
            ProfileExecutors();
        Console.WriteLine("Executors profiling complete.");
        for (var i = 0; i < _iterations; i++)
            ProfileOptimizers();
        Console.WriteLine("Optimizers profiling complete.");
        for (var i = 0; i < _iterations; i++)
            ProfileTokenReader();
        Console.WriteLine("Token Reader profiling complete.");
        for (var i = 0; i < _iterations; i++)
            ProfileFunctionExecutions();
        Console.WriteLine("Function Executions profiling complete.");
    }

    private static void ProfileAstBuilder()
    {
        foreach (var expression in _astBuilderBenchmarks.Expressions)
            _astBuilderBenchmarks.BenchBuildAst(expression);
    }

    private static void ProfileExecutors()
    {
        foreach (var expression in _executorBenchmarks.Expressions)
        {
            _executorBenchmarks.BuildFormula_Unoptimized_DynamicCompiler(expression);
            _executorBenchmarks.BuildFormula_Optimized_DynamicCompiler(expression);
            _executorBenchmarks.BuildFormula_Unoptimized_Interpreter(expression);
            _executorBenchmarks.BuildFormula_Optimized_Interpreter(expression);
        }
    }

    private static void ProfileOptimizers()
    {
        foreach (var expression in _optimizerBenchmarks.Expressions)
        {
            _optimizerBenchmarks.OptimizeOperation_Interpreter(expression);
            #if false
            _optimizerBenchmarks.OptimizeOperation_DynamicCompiler(expression);
            #endif
        }
    }

    private static void ProfileTokenReader()
    {
        foreach (var expression in _tokenReaderBenchmarks.Expressions)
            _tokenReaderBenchmarks.Tokenize(expression);
    }
    private static void ProfileFunctionExecutions()
    {
        foreach (var expression in _functionExecutionBenchmarks.Expressions)
        {
            _functionExecutionBenchmarks.Execute_Dynamic_Unoptimized(expression);
            _functionExecutionBenchmarks.Execute_Dynamic_Optimized(expression);
            _functionExecutionBenchmarks.Execute_Interpreted_Unoptimized(expression);
            _functionExecutionBenchmarks.Execute_Interpreted_Optimized(expression);
        }
    }
}
