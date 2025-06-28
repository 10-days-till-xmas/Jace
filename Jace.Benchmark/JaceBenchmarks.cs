using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Jace.Execution;
using OfficeOpenXml;

namespace Jace.Benchmark;

public static class JaceBenchmarks
{
    private const int NumberOfTests = 1000000;
    private const int NumberOfFunctionsToGenerate = 10000;
    private const int NumberExecutionsPerRandomFunction = 1000;
    
    public static DataTable Benchmark(BenchmarkMode mode, CaseSensitivity caseSensitivity)
    { 
        TimeSpan duration;

        var interpretedEngine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Interpreted,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = false
        });
        var interpretedEngineCaseSensitive = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Interpreted,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = true
        });
        
        var compiledEngine = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Compiled,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = false
        });
        var compiledEngineCaseSensitive = new CalculationEngine(new JaceOptions
        {
            CultureInfo = CultureInfo.InvariantCulture,
            ExecutionMode = ExecutionMode.Compiled,
            CacheEnabled = true,
            OptimizerEnabled = true,
            CaseSensitive = true
        });

        BenchMarkOperation[] benchmarks =
        [
            new() { Formula = "2+3*7", Mode = BenchmarkMode.Static, BenchMarkDelegate = BenchMarkCalculationEngine },
            new() { Formula = "20-3^2", Mode = BenchmarkMode.Static, BenchMarkDelegate = BenchMarkCalculationEngine },
            new() { Formula = "logn(var1, (2+3) * 500)", Mode = BenchmarkMode.SimpleFunction , BenchMarkDelegate = BenchMarkCalculationEngineFunctionBuild },
            new() { Formula = "(var1 + var2 * 3)/(2+3) - something", Mode = BenchmarkMode.Simple , BenchMarkDelegate = BenchMarkCalculationEngineFunctionBuild }
        ];

        var table = new DataTable();
        table.Columns.Add("Engine");
        table.Columns.Add("Case Sensitive");
        table.Columns.Add("Formula");
        table.Columns.Add("Iterations per Random Formula", typeof(int));
        table.Columns.Add("Total Iteration", typeof(int));
        table.Columns.Add("Total Duration");

        foreach (var benchmark in benchmarks)
        {
            if (mode == BenchmarkMode.All || mode == benchmark.Mode)
            {
                if (caseSensitivity is CaseSensitivity.All or CaseSensitivity.CaseInSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(interpretedEngine, benchmark.Formula);
                    table.AddBenchmarkRecord("Interpreted", false, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity is CaseSensitivity.All or CaseSensitivity.CaseSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(interpretedEngineCaseSensitive, benchmark.Formula);
                    table.AddBenchmarkRecord("Interpreted", true, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity is CaseSensitivity.All or CaseSensitivity.CaseInSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(compiledEngine, benchmark.Formula);
                    table.AddBenchmarkRecord("Compiled", false, benchmark.Formula, null, NumberOfTests, duration);
                }

                if (caseSensitivity is CaseSensitivity.All or CaseSensitivity.CaseSensitive)
                {
                    duration = benchmark.BenchMarkDelegate(compiledEngineCaseSensitive, benchmark.Formula);
                    table.AddBenchmarkRecord("Compiled", true, benchmark.Formula, null, NumberOfTests, duration);
                }
            }
        }

        if (mode == BenchmarkMode.All || mode == BenchmarkMode.Random)
        {
            List<string> functions = GenerateRandomFunctions(NumberOfFunctionsToGenerate);

            if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseInSensitive)
            {
                //Interpreted Mode
                duration = BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngine, functions, NumberExecutionsPerRandomFunction);
                table.AddBenchmarkRecord("Interpreted", false, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
                    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);
            }

            if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseSensitive)
            {
                //Interpreted Mode(Case Sensitive)
                duration = BenchMarkCalculationEngineRandomFunctionBuild(interpretedEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);
                table.AddBenchmarkRecord("Interpreted", true, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
                    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);
            }

            if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseInSensitive)
            {
                //Compiled Mode
                duration = BenchMarkCalculationEngineRandomFunctionBuild(compiledEngine, functions, NumberExecutionsPerRandomFunction);
                table.AddBenchmarkRecord("Compiled", false, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
                    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);
            }

            if (caseSensitivity == CaseSensitivity.All || caseSensitivity == CaseSensitivity.CaseSensitive)
            {
                //Compiled Mode(Case Sensitive)
                duration = BenchMarkCalculationEngineRandomFunctionBuild(compiledEngineCaseSensitive, functions, NumberExecutionsPerRandomFunction);
                table.AddBenchmarkRecord("Compiled", true, string.Format("Random Mode {0} functions 3 variables", NumberOfFunctionsToGenerate),
                    NumberExecutionsPerRandomFunction, NumberExecutionsPerRandomFunction * NumberOfFunctionsToGenerate, duration);
            }
        }

        return table;
    }

    private static TimeSpan BenchMarkCalculationEngine(CalculationEngine engine, string functionText)
    {
        DateTime start = DateTime.Now;

        for (int i = 0; i < NumberOfTests; i++)
        {
            engine.Calculate(functionText);
        }

        DateTime end = DateTime.Now;

        return end - start;
    }

    private static TimeSpan BenchMarkCalculationEngineFunctionBuild(CalculationEngine engine, string functionText)
    {
        DateTime start = DateTime.Now;

        Func<int, int, int, double> function = (Func<int, int, int, double>)engine.Formula(functionText)
            .Parameter("var1", DataType.Integer)
            .Parameter("var2", DataType.Integer)
            .Parameter("something", DataType.Integer)
            .Result(DataType.FloatingPoint)
            .Build();

        Random random = new Random();

        for (int i = 0; i < NumberOfTests; i++)
        {
            function(random.Next(), random.Next(), random.Next());
        }

        DateTime end = DateTime.Now;

        return end - start;
    }

    private static List<string> GenerateRandomFunctions(int numberOfFunctions)
    {
        List<string> result = new List<string>();
        FunctionGenerator generator = new FunctionGenerator();

        for (int i = 0; i < numberOfFunctions; i++)
            result.Add(generator.Next());

        return result;
    }

    private static TimeSpan BenchMarkCalculationEngineRandomFunctionBuild(CalculationEngine engine, List<string> functions, 
        int numberOfTests)
    {
        Random random = new Random();

        DateTime start = DateTime.Now;

        Parallel.ForEach(functions,(functionText)=>
        {
            Func<int, int, int, double> function = (Func<int, int, int, double>)engine.Formula(functionText)
                .Parameter("var1", DataType.Integer)
                .Parameter("var2", DataType.Integer)
                .Parameter("var3", DataType.Integer)
                .Result(DataType.FloatingPoint)
                .Build();

            for (int i = 0; i < numberOfTests; i++)
            {
                function(random.Next(), random.Next(), random.Next());
            }
        });

        DateTime end = DateTime.Now;

        return end - start;
    }

    private static void AddBenchmarkRecord(this DataTable table, string engine, bool caseSensitive, string formula, int? iterationsPerRandom, int totalIterations, TimeSpan duration)
    {
        table.Rows.Add(engine, caseSensitive, formula, iterationsPerRandom, totalIterations, duration);
    }

    public static void WriteToExcelFile(DataTable table, string fileName)
    {
        using (var excel = new ExcelPackage())
        {
            var worksheet = excel.Workbook.Worksheets.Add("Results");
            worksheet.Cells.LoadFromDataTable(table, true);

            char endColumnLetter = (char)('A' + table.Columns.Count - 1);

            worksheet.Cells["A1:" + endColumnLetter + "1"].Style.Font.Bold = true;

            for (int i = 0; i < table.Columns.Count; i++)
                worksheet.Column(i + 1).AutoFit();

            excel.SaveAs(new FileInfo(fileName));
        }
    }
}