using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace Jace.Benchmark.Benchmarks;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
[RPlotExporter]
// ReSharper disable once ClassCanBeSealed.Global
public class ConstantFormulaBenchmarks : JaceBenchmarkBase
{
    [Serializable]
    public sealed class FunctionInfo(string function)
    {
        public string Function { get; } = function;
        public Func<int, int, int, double>? CompiledFunction { get; set; }

        public void CompileFunction(CalculationEngine engine)
        {
            CompiledFunction = (Func<int, int, int, double>)engine.Formula(Function)
                                                                  .Parameter("var1", DataType.Integer)
                                                                  .Parameter("var2", DataType.Integer)
                                                                  .Parameter("something", DataType.Integer)
                                                                  .Result(DataType.FloatingPoint)
                                                                  .Build();
        }
        public override string ToString() => Function;
    }

    private readonly Random _random = new();

    public static string[] SimpleFormulae =>
    [
        "2+3*7",
        "20-3^2"
    ];

    public static FunctionInfo[] SimpleFunctionInfos { get; } = [
        new("logn(var1, (2+3) * 500)"),
        new("(var1 + var2 * 3)/(2+3) - something")
    ];

    [GlobalSetup]
    public void GlobalSetup()
    {
        GlobalSetup_Engine();
        foreach (var fi in SimpleFunctionInfos)
        {
            fi.CompileFunction(Engine.Engine);
        }
    }

    [Benchmark]
    [BenchmarkCategory("Static Formula")]
    [ArgumentsSource(nameof(SimpleFormulae))]
    public double SimpleFormulaCalc(string simpleFormula)
    {
        return Engine.Engine.Calculate(simpleFormula);
    }

    [Benchmark]
    [BenchmarkCategory("Simple Function")]
    [ArgumentsSource(nameof(SimpleFunctionInfos))]
    public double EngineFunctionBuildAndRun(FunctionInfo functionInfo)
    {
        var function = (Func<int, int, int, double>)Engine.Engine.Formula(functionInfo.Function)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("something", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();
        return function(_random.Next(), _random.Next(), _random.Next());
    }

    [Benchmark]
    [BenchmarkCategory("Simple Function")]
    [ArgumentsSource(nameof(SimpleFunctionInfos))]
    public Func<int, int, int, double> EngineFunctionBuildOnly(FunctionInfo functionInfo)
    {
        return (Func<int, int, int, double>)Engine.Engine
                                                  .Formula(functionInfo.Function)
                                                  .Parameter("var1", DataType.Integer)
                                                  .Parameter("var2", DataType.Integer)
                                                  .Parameter("something", DataType.Integer)
                                                  .Result(DataType.FloatingPoint)
                                                  .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Simple Function")]
    [ArgumentsSource(nameof(SimpleFunctionInfos))]
    public double EngineFunctionCompiled(FunctionInfo functionInfo)
    {
        return functionInfo.CompiledFunction!(_random.Next(), _random.Next(), _random.Next());
    }
}