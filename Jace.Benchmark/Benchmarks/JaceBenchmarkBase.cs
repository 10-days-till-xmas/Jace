using BenchmarkDotNet.Diagnostics.dotTrace;
using Jace.Execution;

namespace Jace.Benchmark.Benchmarks;
[DotTraceDiagnoser]
[DisassemblyDiagnoser]
public abstract class JaceBenchmarkBase
{
    private static readonly ExpressionInfo[] _expressions =
    [
        new("logn(var1, (2+3) * 500)",
            new ParameterInfo("var1")),
        new("(var1 + var2 * 3)/(2+3) - something",
            new ParameterInfo("var1"),
            new ParameterInfo("var2"),
            new ParameterInfo("something"))
    ];

    public ExpressionInfo[] Expressions { get; } = _expressions;
}