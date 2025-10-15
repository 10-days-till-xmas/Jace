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
    };

    [Params(true, false)]
    public bool CaseSensitive;

    [Params(ExecutionMode.Interpreted, ExecutionMode.Compiled)]
    public ExecutionMode Mode;

    [Params(true, false)]
    public bool CacheEnabled;
    [Params(true, false)]
    public bool OptimizerEnabled;

    protected EngineWrapper Engine { get; set; }

    protected void GlobalSetup_Engine()
    {
        Engine = new EngineWrapper(_baseOptions with
        {
            ExecutionMode = Mode,
            CaseSensitive = CaseSensitive,
            CacheEnabled = CacheEnabled,
            OptimizerEnabled = OptimizerEnabled
        });
    }
}