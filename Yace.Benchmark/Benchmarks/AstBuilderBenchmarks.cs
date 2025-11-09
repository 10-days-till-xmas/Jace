#if BENCHJACE
using Jace.Operations;
using _AstBuilder = Jace.AstBuilder;
#else
using Yace.Operations;
using _AstBuilder = Yace.AstBuilder;
#endif

namespace Yace.Benchmark.Benchmarks;
[BenchmarkCategory]
// ReSharper disable once ClassCanBeSealed.Global
public class AstBuilderBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]

    public Operation BenchBuildAst(ExpressionInfo expressionInfo)
    {
        var astBuilder = new _AstBuilder(expressionInfo.FunctionRegistry,
            expressionInfo.CaseSensitive, expressionInfo.ConstantRegistry);
        return astBuilder.Build(expressionInfo.Tokens);
    }
}
