using System;
using System.Collections.Generic;
using System.Globalization;
using Yace.Tokenizer;

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class TokenReaderBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public object Tokenize(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.Tokenizer.TokenReader(CultureInfo.InvariantCulture)
           .Read(expressionInfo.Expression),
        Library.Yace => new TokenReader(CultureInfo.InvariantCulture)
           .Read(expressionInfo.Expression),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };
}
