using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace Jace.Benchmark.Benchmarks;

[DotTraceDiagnoser]
[DisassemblyDiagnoser]
[RPlotExporter]
public class RandomFormulaBenchmarks : JaceBenchmarkBase
{
    private const int MaxFormulae = 1000;
    private const int RandomSeed = ':' + '3'; // :3 // (109)

    private static string[] _randomFormulae;
    private static int _formulaIndex = 0;

    private static int globalCounter = 0;

    private Random _random;

    private static string RandomFormula => _randomFormulae[++_formulaIndex % MaxFormulae];

    [GlobalSetup]
    public void GlobalSetup()
    {
        globalCounter++;
        GlobalSetup_Engine();
        _random = new Random(RandomSeed);
        _randomFormulae ??= new FunctionGenerator().Next(MaxFormulae).ToArray();

        var formulaDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly()!.Location)
                         ?? throw new InvalidOperationException("Directory not found for the assembly location.");

        Console.WriteLine($"Saving generated formulae to {formulaDir}");

        var formulaeFile = Path.Combine(formulaDir, $"formulae{globalCounter}.txt");

        MoveOldFileIfExists(formulaeFile);

        File.WriteAllLines(formulaeFile, _randomFormulae);
        Console.WriteLine($"Saved {_randomFormulae.Length} random formulae to {formulaeFile}");
        return;

        static void MoveOldFileIfExists(string filePath, int count = 0)
        {
            if (!File.Exists(filePath)) return;
            var newFilePath = Path.ChangeExtension(filePath, $".old{count++}.txt");
            MoveOldFileIfExists(newFilePath, count);
            File.Move(filePath, newFilePath);
            Console.WriteLine($"Moved existing file to {newFilePath}");
        }
    }

    [Benchmark(Description = "Build and execute a random formula")]
    [MaxIterationCount(1000)]
    [BenchmarkCategory("Random Formula")]
    public double RandomFunctionBuild()
    {
        var function = (Func<int, int, int, double>)Engine.Engine.Formula(RandomFormula)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("var3", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();

        return function(_random.Next(), _random.Next(), _random.Next());
    }
}