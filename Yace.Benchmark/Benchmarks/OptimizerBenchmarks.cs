using Yace.Execution;
using Yace.Operations;

namespace Yace.Benchmark.Benchmarks;
// ReSharper disable once ClassCanBeSealed.Global
public class OptimizerBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Operation OptimizeOperation_Interpreter(ExpressionInfo expressionInfo)
    {
        var optimizer = new Optimizer(new Interpreter(expressionInfo.CaseSensitive));
        return optimizer.Optimize(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Operation OptimizeOperation_DynamicCompiler(ExpressionInfo expressionInfo)
    {
        var optimizer = new Optimizer(new DynamicCompiler(expressionInfo.CaseSensitive));
        return optimizer.Optimize(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
    }
}