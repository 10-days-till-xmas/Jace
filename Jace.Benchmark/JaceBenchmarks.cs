using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Jace.Execution;
using static Jace.Benchmark.Utils;

namespace Jace.Benchmark;

internal static class JaceBenchmarks
{
    private const int NumberOfTests = 1000000;
    private const int NumberOfFunctionsToGenerate = 10000;
    private const int NumberExecutionsPerRandomFunction = 1000;

    public static DataTable Benchmark(BenchmarkMode mode, CaseSensitivity caseSensitivity)
    {
        var baseOptions = new JaceOptions
            { CultureInfo = CultureInfo.InvariantCulture, CacheEnabled = true, OptimizerEnabled = true };
        var interpretedEngine = new CalculationEngine(baseOptions with
        {
            ExecutionMode = ExecutionMode.Interpreted, CaseSensitive = false
        });
        var interpretedEngineCaseSensitive = new CalculationEngine(baseOptions with
        {
            ExecutionMode = ExecutionMode.Interpreted, CaseSensitive = true
        });

        var compiledEngine = new CalculationEngine(baseOptions with
        {
            ExecutionMode = ExecutionMode.Compiled, CaseSensitive = false
        });
        var compiledEngineCaseSensitive = new CalculationEngine(baseOptions with
        {
            ExecutionMode = ExecutionMode.Compiled, CaseSensitive = true
        });

        BenchMarkOperation[] benchmarks =
        [
            new() { Formula = "2+3*7", Mode = BenchmarkMode.Static, BenchMarkFunc = BenchMarkCalculationEngine },
            new() { Formula = "20-3^2", Mode = BenchmarkMode.Static, BenchMarkFunc = BenchMarkCalculationEngine },
            new()
            {
                Formula = "logn(var1, (2+3) * 500)", Mode = BenchmarkMode.SimpleFunction,
                BenchMarkFunc = BenchMarkCalculationEngineFunctionBuild
            },
            new()
            {
                Formula = "(var1 + var2 * 3)/(2+3) - something", Mode = BenchmarkMode.Simple,
                BenchMarkFunc = BenchMarkCalculationEngineFunctionBuild
            }
        ];

        var table = new DataTable();
        table.Columns.Add("Engine");
        table.Columns.Add("Case Sensitive");
        table.Columns.Add("Formula");
        table.Columns.Add("Iterations per Random Formula", typeof(int));
        table.Columns.Add("Total Iteration", typeof(int));
        table.Columns.Add("Total Duration");

        foreach (var benchmark in benchmarks) AddDefinedModeBenchmarkEntries(benchmark);

        if (mode.HasFlag(BenchmarkMode.Random)) AddRandomModeBenchmarkEntries();

        return table;

        void AddRandomModeBenchmarkEntries()
        {
            var functions = GenerateRandomFunctions(NumberOfFunctionsToGenerate);

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseInSensitive))
                table.AddBenchmarkRecord("Interpreted",
                    false,
                    $"Random Mode {NumberOfFunctionsToGenerate} functions 3 variables",
                    NumberExecutionsPerRandomFunction,
                    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate,
                    BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngine, functions,
                        NumberExecutionsPerRandomFunction));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseSensitive))
                table.AddBenchmarkRecord("Interpreted",
                    true,
                    $"Random Mode {NumberOfFunctionsToGenerate} functions 3 variables",
                    NumberExecutionsPerRandomFunction,
                    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate,
                    BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngineCaseSensitive,
                        functions,
                        NumberExecutionsPerRandomFunction));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseInSensitive))
                table.AddBenchmarkRecord("Compiled",
                    false,
                    $"Random Mode {NumberOfFunctionsToGenerate} functions 3 variables",
                    NumberExecutionsPerRandomFunction,
                    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate,
                    BenchMarkCalculationEngineRandomFunctionBuild(compiledEngine,
                        functions,
                        NumberExecutionsPerRandomFunction));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseSensitive))
                table.AddBenchmarkRecord("Compiled",
                    true,
                    $"Random Mode {NumberOfFunctionsToGenerate} functions 3 variables",
                    NumberExecutionsPerRandomFunction,
                    NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate,
                    BenchMarkCalculationEngineRandomFunctionBuild(compiledEngineCaseSensitive,
                        functions,
                        NumberExecutionsPerRandomFunction));
        }

        void AddDefinedModeBenchmarkEntries(BenchMarkOperation benchmark)
        {
            if (!mode.HasFlag(benchmark.Mode)) return;
            if (caseSensitivity.HasFlag(CaseSensitivity.CaseInSensitive))
                table.AddBenchmarkRecord("Interpreted",
                    false,
                    benchmark.Formula,
                    null,
                    NumberOfTests,
                    benchmark.BenchMarkFunc(interpretedEngine, benchmark.Formula));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseSensitive))
                table.AddBenchmarkRecord("Interpreted",
                    true,
                    benchmark.Formula,
                    null,
                    NumberOfTests,
                    benchmark.BenchMarkFunc(interpretedEngine, benchmark.Formula));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseInSensitive))
                table.AddBenchmarkRecord("Compiled",
                    false,
                    benchmark.Formula,
                    null,
                    NumberOfTests,
                    benchmark.BenchMarkFunc(compiledEngine, benchmark.Formula));

            if (caseSensitivity.HasFlag(CaseSensitivity.CaseSensitive))
                table.AddBenchmarkRecord("Compiled",
                    true,
                    benchmark.Formula,
                    null,
                    NumberOfTests,
                    benchmark.BenchMarkFunc(compiledEngine, benchmark.Formula));
        }
    }

    private static TimeSpan BenchMarkCalculationEngine(CalculationEngine engine, string functionText)
    {
        return Measure(() =>
            {
                for (var i = 0; i < NumberOfTests; i++) engine.Calculate(functionText);
            }
        );
    }

    private static TimeSpan BenchMarkCalculationEngineFunctionBuild(CalculationEngine engine, string functionText)
    {
        return Measure(() =>
        {
            var function = (Func<int, int, int, double>)engine.Formula(functionText)
                .Parameter("var1", DataType.Integer)
                .Parameter("var2", DataType.Integer)
                .Parameter("something", DataType.Integer)
                .Result(DataType.FloatingPoint)
                .Build();

            var random = new Random();

            for (var i = 0; i < NumberOfTests; i++) function(random.Next(), random.Next(), random.Next());
        });
    }

    private static List<string> GenerateRandomFunctions(int numberOfFunctions)
    {
        return FunctionGenerator.GenerateMany(numberOfFunctions).ToList();
    }

    private static TimeSpan BenchMarkCalculationEngineRandomFunctionBuild(CalculationEngine engine,
        List<string> functions,
        int numberOfTests)
    {
        return Measure(() =>
        {
            var random = new Random();
            Parallel.ForEach(functions, functionText =>
            {
                var function = (Func<int, int, int, double>)engine.Formula(functionText)
                    .Parameter("var1", DataType.Integer)
                    .Parameter("var2", DataType.Integer)
                    .Parameter("var3", DataType.Integer)
                    .Result(DataType.FloatingPoint)
                    .Build();

                for (var i = 0; i < numberOfTests; i++) function(random.Next(), random.Next(), random.Next());
            });
        });
    }


    
}