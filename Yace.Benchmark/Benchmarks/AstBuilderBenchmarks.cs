using Yace.Operations;

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class AstBuilderBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public Operation BuildAst(ExpressionInfo expressionInfo)
    {
        var astBuilder = new AstBuilder(expressionInfo.FunctionRegistry,
            expressionInfo.CaseSensitive, expressionInfo.ConstantRegistry);
        return astBuilder.Build(expressionInfo.Tokens);
    }
}
