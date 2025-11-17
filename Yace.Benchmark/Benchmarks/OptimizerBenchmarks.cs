using Yace.Execution;

using System;

namespace Yace.Benchmark.Benchmarks;
// ReSharper disable once ClassCanBeSealed.Global
public class OptimizerBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public object OptimizeOperation_Interpreter(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.Optimizer(new Jace.Execution.Interpreter(expressionInfo.CaseSensitive))
           .Optimize(expressionInfo.RootOperation_Jace, expressionInfo.FunctionRegistry_Jace,
                expressionInfo.ConstantRegistry_Jace),
        Library.Yace => new Optimizer(new Interpreter(expressionInfo.CaseSensitive))
           .Optimize(expressionInfo.RootOperation, expressionInfo.Context),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };
    #if false // Disabled because it provides no useful data (I was curious)
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public object OptimizeOperation_DynamicCompiler(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.Optimizer(new Jace.Execution.DynamicCompiler(expressionInfo.CaseSensitive))
           .Optimize(expressionInfo.RootOperation_Jace, expressionInfo.FunctionRegistry_Jace,
                expressionInfo.ConstantRegistry_Jace),
        Library.Yace => new Optimizer(new DynamicCompiler(expressionInfo.CaseSensitive))
           .Optimize(expressionInfo.RootOperation, expressionInfo.Context),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };
    #endif
}
