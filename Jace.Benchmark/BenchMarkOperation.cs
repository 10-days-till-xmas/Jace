using System;

namespace Jace.Benchmark;

public class BenchMarkOperation
{
    public string Formula { get; init; }
    public BenchmarkMode Mode { get; init; }
    public Func<CalculationEngine, string, TimeSpan> BenchMarkFunc { get; init; }
}