using System;
using System.CommandLine;
using System.Data;
using System.IO;

namespace Jace.Benchmark;

internal static class Program
{
    private static void Main(string[] args)
    {
        var (fileName, mode, caseSensitivity) = HandleCommandLine(args);
        Console.WriteLine($"Starting benchmark(s) with mode: {mode}, case sensitivity: {caseSensitivity}");
        
        DataTable table = null!;
        var elapsed = Utils.Measure(() =>
        {
            table = JaceBenchmarks.Benchmark(mode, caseSensitivity);
        });

        Utils.WriteToExcelFile(table, fileName);
        Console.WriteLine($"Results written to {fileName}. (Total benchmark time: {elapsed:mm\\:ss\\.fff} ms)");
    }

    private static (string, BenchmarkMode, CaseSensitivity) HandleCommandLine(string[] args)
    {
        var caseSensitivityOption = new Option<CaseSensitivity>("--case-sensitivity", "-c")
        {
            HelpName = "Case Sensitivity",
            DefaultValueFactory = _ => CaseSensitivity.All,
            Description = "Execute in case sensitive mode, case insensitive mode or execute both."
        };
        var modeOption = new Option<BenchmarkMode>("--benchmark-mode", "--mode", "-m")
        {
            HelpName = "Benchmark Mode",
            DefaultValueFactory = _ => BenchmarkMode.All,
            Description = "Specify the benchmark to execute."
        };
        var directoryOption = new Option<string>("--output-dir", "--out", "-o")
        {
            HelpName = "Output directory",
            Description = "The directory to store the output results in.",
            Required = true,
            Arity = ArgumentArity.ExactlyOne
        };

        var rootCommand = new RootCommand("Benchmark Jace Calculation Engine")
        {
            caseSensitivityOption,
            modeOption,
            directoryOption
        };

        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            Console.WriteLine("Error parsing command line arguments:");
            foreach (var error in parseResult.Errors) Console.WriteLine(error);
            Environment.Exit(1);
        }

        var dirName = parseResult.GetRequiredValue(directoryOption);
        var mode = parseResult.GetValue(modeOption);
        var caseSensitivity = parseResult.GetValue(caseSensitivityOption);
        if (string.IsNullOrEmpty(dirName))
        {
            Console.WriteLine("Directory name is required.");
            Environment.Exit(1);
        }

        var fileName = Path.Combine(dirName, $@"JaceBenchmarkResults-{DateTime.Now:yyyy\.MM\.dd\-HHmmss}.xlsx");

        return (fileName, mode, caseSensitivity);
    }
}