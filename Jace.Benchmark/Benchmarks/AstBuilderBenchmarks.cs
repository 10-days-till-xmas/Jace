using Jace.Operations;

namespace Jace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class AstBuilderBenchmarks : JaceBenchmarkBase
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