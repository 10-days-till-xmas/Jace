#if BENCHJACE
using Jace.Execution;
using Jace.Operations;
#else
using Yace.Execution;
using Yace.Operations;
#endif
namespace Yace.Benchmark.Benchmarks;
// ReSharper disable once ClassCanBeSealed.Global
public class OptimizerBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Operation OptimizeOperation_Interpreter(ExpressionInfo expressionInfo)
    {
        #if BENCHJACE
        var optimizer = new Jace.Optimizer(new Interpreter(expressionInfo.CaseSensitive));
        #else
        var optimizer = new Optimizer(new Interpreter(expressionInfo.CaseSensitive));
        #endif
        return optimizer.Optimize(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Operation OptimizeOperation_DynamicCompiler(ExpressionInfo expressionInfo)
    {
        #if BENCHJACE
        var optimizer = new Jace.Optimizer(new DynamicCompiler(expressionInfo.CaseSensitive));
        #else
        var optimizer = new Optimizer(new DynamicCompiler(expressionInfo.CaseSensitive));
        #endif
        return optimizer.Optimize(expressionInfo.RootOperation,
            expressionInfo.FunctionRegistry,
            expressionInfo.ConstantRegistry);
    }
}
