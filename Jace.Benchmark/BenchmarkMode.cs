using System;

namespace Jace.Benchmark;

[Flags]
public enum BenchmarkMode
{
    Static = 1,
    Simple = 2,
    SimpleFunction = 4,
    Random = 8,
    All = Static | Simple | SimpleFunction | Random
}