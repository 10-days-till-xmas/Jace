using System.Collections.Generic;
using System.Globalization;
using Yace.Tokenizer;

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class TokenReaderBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public List<Token> Tokenize(ExpressionInfo expressionInfo)
    {
        var tokenizer = new TokenReader(CultureInfo.InvariantCulture);
        return tokenizer.Read(expressionInfo.Expression);
    }
}