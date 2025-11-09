using Yace.Execution;

namespace Yace.Benchmark.Unused;

public sealed class EngineWrapper(YaceOptions options)
{
    public CalculationEngine Engine { get; } = new(options);
    private ExecutionMode ExecutionMode { get; } = options.ExecutionMode;
    private bool CaseSensitivity { get; } = options.CaseSensitive;

    public static implicit operator CalculationEngine(EngineWrapper wrapper)
    {
        return wrapper.Engine;
    }

    public override string ToString()
    {
        return $"{ExecutionMode}, {(CaseSensitivity ? "CS:t" : "CS:f")}";
    }
}
