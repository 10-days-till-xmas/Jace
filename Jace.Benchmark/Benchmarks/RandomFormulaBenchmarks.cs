using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace Jace.Benchmark.Benchmarks;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
[RPlotExporter]
// ReSharper disable once ClassCanBeSealed.Global
public class RandomFormulaBenchmarks : JaceBenchmarkBase
{
    private const int MaxFormulae = 1000;
    private const int RandomSeed = ':' + '3'; // :3 // (109)

    private static string[]? _randomFormulae = null!;
    private static Func<int, int, int, double>[]? _randomFormulaeCompiled = null!;
    private static int _formulaIndex = 0;

    private static int globalCounter = 0;

    private Random? _random = null!;

    private static string RandomFormula => _randomFormulae![++_formulaIndex % MaxFormulae];

    private static Func<int, int, int, double> RandomFormulaCompiled => _randomFormulaeCompiled![++_formulaIndex % MaxFormulae];

    [GlobalSetup]
    public void GlobalSetup()
    {
        globalCounter++;
        GlobalSetup_Engine();
        _random = new Random(RandomSeed);
        _randomFormulae ??= new FunctionGenerator().Next(MaxFormulae).ToArray();
        _randomFormulaeCompiled ??= _randomFormulae.Select(f => Engine.Engine.Formula(f)
                                                                     .Parameter("var1", DataType.Integer)
                                                                     .Parameter("var2", DataType.Integer)
                                                                     .Parameter("var3", DataType.Integer)
                                                                     .Result(DataType.FloatingPoint)
                                                                     .Build())
                                                  .Cast<Func<int, int, int, double>>()
                                                  .ToArray();
        var formulaDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                         ?? throw new InvalidOperationException("Directory not found for the assembly location.");

        Console.WriteLine($"Saving generated formulae to {formulaDir}");

        var formulaeFile = Path.Combine(formulaDir, $"formulae-run{globalCounter}.txt");
        var counter = 1;
        while (File.Exists(formulaeFile))
        {
            formulaeFile = Path.Combine(formulaDir, $"formulae-run{globalCounter} ({counter}).txt");
            counter++;
        }

        File.WriteAllLines(formulaeFile, _randomFormulae);
        Console.WriteLine($"Saved {_randomFormulae.Length} random formulae to {formulaeFile}");
    }

    [Benchmark]
    [MaxIterationCount(1000)]
    [BenchmarkCategory("Random Formula")]
    public Func<int, int, int, double> RandomFunctionBuildOnly()
    {
        return (Func<int, int, int, double>)Engine.Engine
                                                  .Formula(RandomFormula)
                                                  .Parameter("var1", DataType.Integer)
                                                  .Parameter("var2", DataType.Integer)
                                                  .Parameter("var3", DataType.Integer)
                                                  .Result(DataType.FloatingPoint)
                                                  .Build();
    }

    [Benchmark]
    [MaxIterationCount(1000)]
    [BenchmarkCategory("Random Formula")]
    public double RandomFunctionRunCompiled()
    {
        return RandomFormulaCompiled(_random!.Next(), _random.Next(), _random.Next());
    }

    [Benchmark]
    [MaxIterationCount(1000)]
    [BenchmarkCategory("Random Formula")]
    public double RandomFunctionBuildAndRun()
    {
        var function = (Func<int, int, int, double>)Engine.Engine.Formula(RandomFormula)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("var3", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();

        return function(_random!.Next(), _random.Next(), _random.Next());
    }

    [Benchmark]
    [MaxIterationCount(1000)]
    [BenchmarkCategory("Random Formula")]
    public double RandomFunctionCalculate()
    {
        return Engine.Engine.Calculate(RandomFormula, new Dictionary<string, double>()
        {
            { "var1", _random!.Next() },
            { "var2", _random.Next() },
            { "var3", _random.Next() }
        });
    }
}