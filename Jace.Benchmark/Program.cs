using System;
using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Running;

namespace Jace.Benchmark;

internal static class Program
{
    private static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found.");

        BenchmarkRunner.Run(assembly, args: args);
    }
}