using System;
using System.IO;
using System.CommandLine;
using static Jace.Benchmark.JaceBenchmarks;

namespace Jace.Benchmark;

static class Program
{
    private static (string filePath, BenchmarkMode benchmarkMode, CaseSensitivity caseSensitivity) HandleCommandLine(string[] args)
    {
        var rootCommand = new RootCommand("Benchmark Jace Calculation Engine");
        var caseSensitivityOption = new Option<CaseSensitivity>("--case-sensitivity",
            "-c")
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
        
        var directoryOption = new Option<string>("--filename", "--file", "-f")
        {
            HelpName = "file",
            Description = "The file to store the output results in.",
            Required = true,
            Arity = ArgumentArity.ExactlyOne
        };
        rootCommand.Add(caseSensitivityOption);
        rootCommand.Add(modeOption);
        rootCommand.Add(directoryOption);
        rootCommand.Description = "Benchmark Jace Calculation Engine";
        
        var parseResult = rootCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            Console.WriteLine("Error parsing command line arguments:");
            foreach (var error in parseResult.Errors)
            {
                Console.WriteLine(error);
            }
            Environment.Exit(1);
        }
        var dirName = parseResult.GetValue(directoryOption);
        var mode = parseResult.GetValue(modeOption);
        var caseSensitivity = parseResult.GetValue(caseSensitivityOption);
        if (string.IsNullOrEmpty(dirName))
        {
            Console.WriteLine("Directory name is required.");
            Environment.Exit(1);
        }
        var fileName = Path.Combine(dirName, "JaceBenchmarkResults" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        
        return (fileName, mode, caseSensitivity);
    }
    
    private static void Main(string[] args)
    {
        var (fileName, mode, caseSensitivity) = HandleCommandLine(args);
        
        var table = JaceBenchmarks.Benchmark(mode, caseSensitivity);
        WriteToExcelFile(table, fileName);
        Console.WriteLine("Results written to " + fileName);
    }
}