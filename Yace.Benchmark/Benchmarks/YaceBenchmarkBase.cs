using BenchmarkDotNet.Diagnostics.dotTrace;
#if BENCHJACE
using Jace.Execution;
using _DataType = Jace.DataType;
#else
using Yace.Execution;
using _DataType = Yace.DataType;
#endif

namespace Yace.Benchmark.Benchmarks;
[DotTraceDiagnoser]
[DisassemblyDiagnoser]
public abstract class YaceBenchmarkBase
{
    private static readonly ExpressionInfo[] _expressions =
    [
        new("logn(var1, (2+3) * 500)",
            new ParameterInfo { Name="var1", DataType = _DataType.FloatingPoint}),
        new("(var1 + var2 * 3)/(2+3) - something",
            new ParameterInfo { Name="var1", DataType = _DataType.FloatingPoint },
            new ParameterInfo { Name="var2", DataType = _DataType.FloatingPoint },
            new ParameterInfo { Name="something", DataType = _DataType.FloatingPoint })
    ];

    public ExpressionInfo[] Expressions { get; } = _expressions;
}
