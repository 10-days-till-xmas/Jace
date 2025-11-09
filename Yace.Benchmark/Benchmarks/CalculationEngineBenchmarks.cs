using Yace.Execution;
#if BENCHJACE
using ExecutionMode = Jace.Execution.ExecutionMode;
#else
using ExecutionMode = Yace.Execution.ExecutionMode;
#endif
namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class CalculationEngineBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Calculate_DynamicCompiler(ExpressionInfo expressionInfo)
    {
        #if BENCHJACE
        var engine = new Jace.CalculationEngine(new Jace.JaceOptions()
        #else
        var engine = new CalculationEngine(new YaceOptions
        #endif
        {
            CaseSensitive = true,
            ExecutionMode = ExecutionMode.Compiled,
            DefaultConstants = true,
            DefaultFunctions = true
        });
        return engine.Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Calculate_Interpreter(ExpressionInfo expressionInfo)
    {
        #if BENCHJACE
        var engine = new Jace.CalculationEngine(new Jace.JaceOptions()
        #else
        var engine = new CalculationEngine(new YaceOptions
        #endif
        {
            CaseSensitive = true,
            ExecutionMode = ExecutionMode.Interpreted,
            DefaultConstants = true,
            DefaultFunctions = true
        });
        return engine.Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary);
    }
}
