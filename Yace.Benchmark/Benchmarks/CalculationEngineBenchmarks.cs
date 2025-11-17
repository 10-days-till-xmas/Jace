
using System;

namespace Yace.Benchmark.Benchmarks;

// ReSharper disable once ClassCanBeSealed.Global
public class CalculationEngineBenchmarks : YaceBenchmarkBase
{
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Calculate_DynamicCompiler(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.CalculationEngine(new Jace.JaceOptions()
        {
            CaseSensitive = true,
            ExecutionMode = Jace.Execution.ExecutionMode.Compiled,
            DefaultConstants = true,
            DefaultFunctions = true
        }).Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary),
        Library.Yace => new CalculationEngine(new YaceOptions
        {
            CaseSensitive = true,
            ExecutionMode = Execution.ExecutionMode.Compiled,
            DefaultConstants = true,
            DefaultFunctions = true
        }).Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };

    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public double Calculate_Interpreter(ExpressionInfo expressionInfo) => expressionInfo.Library switch
    {
        Library.Jace => new Jace.CalculationEngine(new Jace.JaceOptions()
        {
            CaseSensitive = true,
            ExecutionMode = Jace.Execution.ExecutionMode.Interpreted,
            DefaultConstants = true,
            DefaultFunctions = true
        }).Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary),
        Library.Yace => new CalculationEngine(new YaceOptions
        {
            CaseSensitive = true,
            ExecutionMode = Execution.ExecutionMode.Interpreted,
            DefaultConstants = true,
            DefaultFunctions = true
        }).Calculate(expressionInfo.Expression, expressionInfo.SimpleParameterDictionary),
        _ => throw new NotSupportedException($"Library {expressionInfo.Library} is not supported.")
    };
}
