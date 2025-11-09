#if false // Doesn't provide meaningful information
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Diagnostics.dotTrace;
using Yace.Execution;

namespace Yace.Benchmark;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
// ReSharper disable once ClassCanBeSealed.Global
// ReSharper disable UnassignedField.Global
public class RandomFormulaBenchmarks
{
    private const int MaxFormulae = 1000;
    private const int RandomSeed = ':' + '3'; // :3 // (109)

    private static string[] _randomFormulae = null!;
    private static Func<int, int, int, double>[] _randomFormulaeCompiled = null!;
    private static int _formulaIndex = 0;

    private static int globalCounter = 0;

    private readonly Random _random = new(RandomSeed);

    private static string RandomFormula => _randomFormulae[++_formulaIndex % MaxFormulae];

    private static Func<int, int, int, double> RandomFormulaCompiled => _randomFormulaeCompiled[++_formulaIndex % MaxFormulae];

    private static readonly YaceOptions _baseOptions = new(CultureInfo.InvariantCulture);

    [Params(true, false)]
    public bool CaseSensitive;
    [Params(ExecutionMode.Interpreted, ExecutionMode.Compiled)]
    public ExecutionMode Mode;
    [Params(true, false)]
    public bool CacheEnabled;
    [Params(true, false)]
    public bool OptimizerEnabled;
    private EngineWrapper Engine { get; set; } = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        globalCounter++;
        Engine = new EngineWrapper(_baseOptions with
        {
            ExecutionMode = Mode,
            CaseSensitive = CaseSensitive,
            CacheEnabled = CacheEnabled,
            OptimizerEnabled = OptimizerEnabled
        });
        _randomFormulae = FunctionGenerator.GenerateMany(MaxFormulae)
                                           .ToArray();
        _randomFormulaeCompiled = _randomFormulae.Select(f => Engine.Engine.Formula(f)
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
        for (var counter = 1; File.Exists(formulaeFile); counter++)
        {
            formulaeFile = Path.Combine(formulaDir, $"formulae-run{globalCounter} ({counter}).txt");
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
        return RandomFormulaCompiled(_random.Next(), _random.Next(), _random.Next());
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

        return function(_random.Next(), _random.Next(), _random.Next());
    }

    [Benchmark]
    [MaxIterationCount(MaxFormulae)]
    [BenchmarkCategory("Random Formula")]
    public double RandomFunctionCalculate()
    {
        return Engine.Engine.Calculate(RandomFormula, new Dictionary<string, double>()
        {
            { "var1", _random.Next() },
            { "var2", _random.Next() },
            { "var3", _random.Next() }
        });
    }
}
#endif
