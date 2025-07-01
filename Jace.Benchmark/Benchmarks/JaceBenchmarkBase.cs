using System.Globalization;
using BenchmarkDotNet.Attributes;
using Jace.Execution;

// ReSharper disable UnassignedField.Global

namespace Jace.Benchmark.Benchmarks;

public abstract class JaceBenchmarkBase
{
    private static readonly JaceOptions _baseOptions = new()
    {
        CultureInfo = CultureInfo.InvariantCulture,
        CacheEnabled = true,
        OptimizerEnabled = true
    };

    [Params(true, false)] public bool CaseSensitive;

    [Params(ExecutionMode.Interpreted, ExecutionMode.Compiled)]
    public ExecutionMode Mode;

    protected EngineWrapper Engine { get; set; }

    protected void GlobalSetup_Engine()
    {
        Engine = new EngineWrapper(_baseOptions with
        {
            ExecutionMode = Mode,
            CaseSensitive = CaseSensitive
        });
    }
}