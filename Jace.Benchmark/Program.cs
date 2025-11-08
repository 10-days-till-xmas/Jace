using System;
using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Xml;
using BenchmarkDotNet.Running;
using Jace.Benchmark;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
// dotnet run -c Release --project Jace.Benchmark
if (args is ["profile"])
{
    // Run with profiling. No BenchmarkDotNet features.
    Profiler.RunClean();
    Console.WriteLine("Profiling complete.");
    return;
}

var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found.");
IConfig config = DefaultConfig.Instance
                              .AddDiagnoser(
                                   MemoryDiagnoser.Default,
                                   ThreadingDiagnoser.Default,
                                   new DotTraceDiagnoser(),
                                   new DisassemblyDiagnoser(
                                       new DisassemblyDiagnoserConfig(
                                           printSource: true,
                                           printInstructionAddresses: false,
                                           exportHtml: true,
                                           exportCombinedDisassemblyReport: true,
                                           exportDiff: true)))
                              .AddExporter(HtmlExporter.Default,
                                   XmlExporter.Default)
                              .WithOption(ConfigOptions.JoinSummary, true)
                              .AddColumn(StatisticColumn.AllStatistics)
                              .WithWakeLock(WakeLockType.Display);
BenchmarkRunner.Run(assembly, config, args);