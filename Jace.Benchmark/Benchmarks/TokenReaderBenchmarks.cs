using System.Collections.Generic;
using System.Globalization;
using Jace.Tokenizer;

namespace Jace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class TokenReaderBenchmarks : JaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public List<Token> Tokenize(ExpressionInfo expressionInfo)
    {
        var tokenizer = new TokenReader(CultureInfo.InvariantCulture);
        return tokenizer.Read(expressionInfo.Expression);
    }
}