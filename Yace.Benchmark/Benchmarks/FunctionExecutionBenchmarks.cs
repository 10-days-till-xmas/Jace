using System;

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class FunctionExecutionBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Execute_Dynamic_Unoptimized(ExpressionInfo expressionInfo) =>
        expressionInfo.CompiledFunction_Dynamic switch
        {
            Func<double> f0 => f0(),
            Func<double, double> f1 => f1(1.0),
            Func<double, double, double> f2 => f2(1.0, 2.0),
            Func<double, double, double, double> f3 => f3(1.0, 2.0, 3.0),
            _ => throw new InvalidOperationException("Unsupported function signature.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Execute_Dynamic_Optimized(ExpressionInfo expressionInfo) =>
        expressionInfo.CompiledFunction_Dynamic_Optimized switch
        {
            Func<double> f0 => f0(),
            Func<double, double> f1 => f1(1.0),
            Func<double, double, double> f2 => f2(1.0, 2.0),
            Func<double, double, double, double> f3 => f3(1.0, 2.0, 3.0),
            _ => throw new InvalidOperationException("Unsupported function signature.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Execute_Interpreted_Unoptimized(ExpressionInfo expressionInfo) =>
        expressionInfo.CompiledFunction_Interpreted switch
        {
            Func<double> f0 => f0(),
            Func<double, double> f1 => f1(1.0),
            Func<double, double, double> f2 => f2(1.0, 2.0),
            Func<double, double, double, double> f3 => f3(1.0, 2.0, 3.0),
            _ => throw new InvalidOperationException("Unsupported function signature.")
        };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Execute_Interpreted_Optimized(ExpressionInfo expressionInfo) =>
        expressionInfo.CompiledFunction_Interpreted_Optimized switch
        {
            Func<double> f0 => f0(),
            Func<double, double> f1 => f1(1.0),
            Func<double, double, double> f2 => f2(1.0, 2.0),
            Func<double, double, double, double> f3 => f3(1.0, 2.0, 3.0),
            _ => throw new InvalidOperationException("Unsupported function signature.")
        };
}
