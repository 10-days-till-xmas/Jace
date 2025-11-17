using System;

namespace Yace.Benchmark.Benchmarks;

[BenchmarkCategory]
// ReSharper disable once ClassCanBeSealed.Global
public class AstBuilderBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]

    public object BenchBuildAst(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.AstBuilder(expressionInfo.FunctionRegistry_Jace,
                expressionInfo.CaseSensitive, expressionInfo.ConstantRegistry_Jace)
           .Build(expressionInfo.Tokens_Jace),
        Library.Yace => new AstBuilder(expressionInfo.FunctionRegistry,
                expressionInfo.CaseSensitive, expressionInfo.ConstantRegistry)
           .Build(expressionInfo.Tokens),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };
}
