using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace Jace.Benchmark.Benchmarks;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
[RPlotExporter]
public sealed class ConstantFormulaBenchmarks : JaceBenchmarkBase
{
    private readonly Random _random = new();

    public static IEnumerable<string> SimpleFormulae =>
    [
        "2+3*7",
        "20-3^2"
    ];

    public static IEnumerable<string> SimpleFunctions =>
    [
        "logn(var1, (2+3) * 500)",
        "(var1 + var2 * 3)/(2+3) - something"
    ];

    public static Func<int, int, int, double>[] SimpleFunctionsCompiled { get; private set; }

    [GlobalSetup]
    public void GlobalSetup()
    {
        GlobalSetup_Engine();
        SimpleFunctionsCompiled = SimpleFunctions.Select(f => Engine.Engine.Formula(f)
                                                                    .Parameter("var1", DataType.Integer)
                                                                    .Parameter("var2", DataType.Integer)
                                                                    .Parameter("something", DataType.Integer)
                                                                    .Result(DataType.FloatingPoint)
                                                                    .Build())
                                                 .Cast<Func<int, int, int, double>>()
                                                 .ToArray(); // Force immediate evaluation
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
    [ArgumentsSource(nameof(SimpleFunctions))]
    public double EngineFunctionBuildAndRun(string simpleFunction)
    {
        var function = (Func<int, int, int, double>)Engine.Engine.Formula(simpleFunction)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("something", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();
        return function(_random.Next(), _random.Next(), _random.Next());
    }

    [Benchmark]
    [BenchmarkCategory("Simple Function")]
    [ArgumentsSource(nameof(SimpleFunctions))]
    public Func<int, int, int, double> EngineFunctionBuildOnly(string simpleFunction)
    {
        return (Func<int, int, int, double>)Engine.Engine
                                                  .Formula(simpleFunction)
                                                  .Parameter("var1", DataType.Integer)
                                                  .Parameter("var2", DataType.Integer)
                                                  .Parameter("something", DataType.Integer)
                                                  .Result(DataType.FloatingPoint)
                                                  .Build();
    }

    [Benchmark]
    [BenchmarkCategory("Simple Function")]
    [ArgumentsSource(nameof(SimpleFunctionsCompiled))]
    public double EngineFunctionCompiled(Func<int, int, int, double> function)
    {
        return function(_random.Next(), _random.Next(), _random.Next());
    }
}