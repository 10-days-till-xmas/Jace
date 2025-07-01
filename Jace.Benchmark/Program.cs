using System;
using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Jace.Benchmark;

internal static class Program
{
    private static void Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        Console.WriteLine("No command line arguments provided. Running benchmarks with default settings.");
        var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found.");

        BenchmarkRunner.Run(assembly);
    }
}