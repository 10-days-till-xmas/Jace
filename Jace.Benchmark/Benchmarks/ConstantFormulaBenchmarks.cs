using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace Jace.Benchmark.Benchmarks;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
[RPlotExporter]
public class ConstantFormulaBenchmarks : JaceBenchmarkBase
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

    [GlobalSetup]
    public void GlobalSetup()
    {
        GlobalSetup_Engine();
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
    public double EngineFunctionBuild(string simpleFunction)
    {
        var function = (Func<int, int, int, double>)Engine.Engine.Formula(simpleFunction)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("something", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();
        return function(_random.Next(), _random.Next(), _random.Next());
    }
}