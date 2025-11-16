using System;
using System.Globalization;
using System.Reflection;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.dotTrace;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Exporters.Xml;
using BenchmarkDotNet.Running;
using Yace.Benchmark;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
// dotnet run -c Release --project Yace.Benchmark
if (args is ["profile"])
{
    // Run with profiling. No BenchmarkDotNet features.
    Profiler.RunClean();
    Console.WriteLine("Profiling complete.");
    return;
}

var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Entry assembly not found.");
IConfig config = DefaultConfig.Instance
                              .AddDiagnoser(MemoryDiagnoser.Default)
                              .AddDiagnoser(ThreadingDiagnoser.Default)
                              .AddDiagnoser(new DotTraceDiagnoser())
                              .AddDiagnoser(new DisassemblyDiagnoser(new DisassemblyDiagnoserConfig(
                                   printSource: true,
                                   printInstructionAddresses: false,
                                   exportHtml: true,
                                   exportCombinedDisassemblyReport: true,
                                   exportDiff: true)))
                              .AddExporter(HtmlExporter.Default)
                              .AddExporter(MarkdownExporter.GitHub)
                              .AddExporter(XmlExporter.Default)
                              .AddExporter(JsonExporter.Default)
                              .WithOption(ConfigOptions.JoinSummary, true)
                              .AddColumn(StatisticColumn.AllStatistics)
                               #if BENCHJACE
                              .AddJob(Job.Default.WithCustomBuildConfiguration("BenchJace"))
                               #endif
                              .WithWakeLock(WakeLockType.Display);
BenchmarkRunner.Run(assembly, config, args);
